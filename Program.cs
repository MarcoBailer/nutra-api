using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nutra.Data;
using Nutra.Enum;
using Nutra.Helper;
using Nutra.Interfaces;
using Nutra.Middleware;
using Nutra.Models.Usuario;
using Nutra.Seeder;
using Nutra.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var myNextAppPolicy = "_myNextAppPolicy";

var authSettings = builder.Configuration.GetSection("Authentication");
var authority = authSettings["Authority"] ?? throw new InvalidOperationException("Authentication:Authority não configurado.");
var audience = authSettings["Audience"] ?? throw new InvalidOperationException("Authentication:Audience não configurado.");

var connectionString = builder.Configuration
    ["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContextFactory<AlimentosContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AlimentosContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
    options.DefaultScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = authority;
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = authority,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        NameClaimType = "name",
        RoleClaimType = "role",
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    // Configuração para desenvolvimento (ignorar SSL)
    options.BackchannelHttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var authLogger = context.HttpContext.RequestServices.GetRequiredService<AuthLogger>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            
            authLogger.LogException("JWT-AUTH-FAILED", context.Exception, "ANONYMOUS");
            logger.LogError(context.Exception, $"[NutraFoodApi JWT] Falha na validação do token: {context.Exception.Message}");
            
            return Task.CompletedTask;
        },
        OnTokenValidated = async context =>
        {
            var authLogger = context.HttpContext.RequestServices.GetRequiredService<AuthLogger>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            
            if (context.Principal?.Identity is ClaimsIdentity identity)
            {
                var userId = context.Principal?.FindFirst("sub")?.Value
                    ?? context.Principal?.FindFirst("client_id")?.Value
                    ?? "UNKNOWN";
                
                authLogger.LogOpenIdEvent("JWT-TOKEN-VALIDATED", $"Bearer token validado", userId);

                var userManager = context.HttpContext.RequestServices
                    .GetRequiredService<UserManager<ApplicationUser>>();

                var localUser = await EnsureLocalUserProjectionAsync(context.Principal!, userManager, logger);

                var existing = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (existing != null)
                {
                    identity.RemoveClaim(existing);
                }

                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, localUser.Id));
                identity.AddClaim(new Claim("local_user_id", localUser.Id));
            }
            else
            {
                logger.LogWarning("[NutraFoodApi JWT] Principal ou Identity é nulo!");
            }
        }
    };
});


// AUTORIZAÇÃO: Configura política padrão que requer qualquer usuário autenticado
builder.Services.AddAuthorization(options =>
{
    var bearerPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("Bearer")
        .Build();

    options.DefaultPolicy = bearerPolicy;
    options.FallbackPolicy = bearerPolicy;
});


// CORS: Usa a URL do frontend configurada em AppSettings ou variável de ambiente
// Em Docker, AppSettings__BaseUrlFront é injetado via env var no docker-compose
var frontendUrl = builder.Configuration["AppSettings:BaseUrlFront"] ?? "http://localhost:3000";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myNextAppPolicy,
        policy => policy
            .WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod());
});


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddScoped<IBusca, BuscaService>();
builder.Services.AddScoped<ICalculadoraNutricional, CalculadoraNutricionalService>();
builder.Services.AddScoped<IUserProfile, UserProfileService>();
builder.Services.AddScoped<IAccounts, AccountsService>();
builder.Services.AddScoped<INutricionista, NutricionistaService>();
builder.Services.AddScoped<IRefeicao, RefeicaoService>();
builder.Services.AddScoped<IAvaliacaoNutricional, AvaliacaoNutricionalService>();
builder.Services.AddScoped<IPlanoAlimentar, PlanoAlimentarService>();
builder.Services.AddScoped<IDiarioAlimentar, DiarioAlimentarService>();

// ===== LOGGING DE AUTENTICAÇÃO =====
// Registrado como Singleton pois é usado em eventos OpenID (fora de escopo HTTP)
builder.Services.AddSingleton<AuthLogger>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==================================================================
// AUTO-MIGRATION (PARA DOCKER)
// ==================================================================
// Aplica migrations pendentes automaticamente no startup
using (var migrationScope = app.Services.CreateScope())
{
    var services = migrationScope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Verificando migrations pendentes...");
        var dbContext = services.GetRequiredService<AlimentosContext>();
        
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            logger.LogInformation("Aplicando migrations...");
            dbContext.Database.Migrate();
            logger.LogInformation("Migrations aplicadas com sucesso.");
        }
        else
        {
            logger.LogInformation("Nenhuma migration pendente.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao aplicar migrations.");
    }
}

// ==================================================================
// FORWARDED HEADERS (OBRIGATÓRIO PARA DOCKER/REVERSE PROXY)
// ==================================================================
// IMPORTANTE: Limpar KnownProxies/Networks para aceitar headers de qualquer proxy
// Em produção com Docker/Nginx/Tailscale, o proxy não é localhost
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor 
                     | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

// ==================================================================
// PATH BASE (usa apenas em Docker, onde a app é montada em /nutra-api/)
// ==================================================================
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOCKER_CONTAINER")))
{
    app.UsePathBase("/nutra-api");
}

// Swagger habilitado em todos os ambientes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nutra API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors(myNextAppPolicy);

app.UseAuthentication();
app.UseAuthorization();

// ===== MIDDLEWARE DE LOGGING DE AUTENTICAÇÃO =====
// DEVE vir APÓS UseAuthentication para capturar o usuário autenticado
app.UseAuthLogging();

app.MapControllers();

// Health check endpoint (usado pelo Docker healthcheck)
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", service = "nutra-api" }))
   .AllowAnonymous();

// Seed das tabelas de alimentos (executa apenas uma vez, se as tabelas estiverem vazias)
using (var seedScope = app.Services.CreateScope())
{
    var logger = seedScope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Seed de Roles do Identity
    try
    {
        var roleManager = seedScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = ["Paciente", "Nutricionista", "Admin"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Role '{Role}' criada com sucesso.", role);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao criar roles do Identity.");
    }

    // Seed de Alimentos
    try
    {
        await DatabaseSeeder.SeedAsync(app.Services, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao executar o seed das tabelas de alimentos.");
    }
}

app.Run();

static async Task<ApplicationUser> EnsureLocalUserProjectionAsync(
   ClaimsPrincipal principal,
   UserManager<ApplicationUser> userManager,
   ILogger logger)
{
   var subject = principal.FindFirstValue("sub");
   var email = principal.FindFirstValue("email");
   var displayName = principal.FindFirstValue("name")
       ?? principal.FindFirstValue(ClaimTypes.Name)
       ?? email
       ?? subject
       ?? "Usuário";

   if (string.IsNullOrWhiteSpace(email))
   {
       if (string.IsNullOrWhiteSpace(subject))
       {
           throw new SecurityTokenValidationException("Token não contém nem email nem sub.");
       }

       email = $"{subject}@sso.local";
   }

   var roles = principal.FindAll("role")
       .Select(claim => claim.Value)
       .Where(value => !string.IsNullOrWhiteSpace(value))
       .Distinct(StringComparer.OrdinalIgnoreCase)
       .ToArray();

   var resolvedRole = ResolvePrimaryRole(roles);
   var user = await userManager.FindByEmailAsync(email);

   if (user == null)
   {
       user = new ApplicationUser
       {
           UserName = email,
           Email = email,
           NomeCompleto = displayName,
           CPF = string.Empty,
           Role = resolvedRole,
           EmailConfirmed = true,
           SecurityStamp = Guid.NewGuid().ToString()
       };

       var createResult = await userManager.CreateAsync(user);
       if (!createResult.Succeeded)
       {
           var errors = string.Join(", ", createResult.Errors.Select(error => error.Description));
           throw new SecurityTokenValidationException($"Falha ao criar projeção local do usuário: {errors}");
       }

       logger.LogInformation("[NutraFoodApi JWT] Usuário local criado para {Email}", email);
   }
   else
   {
       var dirty = false;

       if (!string.Equals(user.NomeCompleto, displayName, StringComparison.Ordinal))
       {
           user.NomeCompleto = displayName;
           dirty = true;
       }

       if (user.Role != resolvedRole)
       {
           user.Role = resolvedRole;
           dirty = true;
       }

       if (dirty)
       {
           user.AtualizadoEm = DateTime.UtcNow;
           await userManager.UpdateAsync(user);
       }
   }

   var currentRoles = await userManager.GetRolesAsync(user);
   var targetRoles = roles.Length > 0 ? roles : [resolvedRole.ToString()];
   var rolesToRemove = currentRoles.Except(targetRoles, StringComparer.OrdinalIgnoreCase).ToArray();
   var rolesToAdd = targetRoles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToArray();

   if (rolesToRemove.Length > 0)
   {
       await userManager.RemoveFromRolesAsync(user, rolesToRemove);
   }

   if (rolesToAdd.Length > 0)
   {
       await userManager.AddToRolesAsync(user, rolesToAdd);
   }

   return user;
}

static ETipoRole ResolvePrimaryRole(IEnumerable<string> roles)
{
    var roleSet = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);

    if (roleSet.Contains("Admin"))
    {
        return ETipoRole.Admin;
    }

    if (roleSet.Contains("Nutricionista"))
    {
        return ETipoRole.Nutricionista;
    }

    return ETipoRole.Paciente;
}
