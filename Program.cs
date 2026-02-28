using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.Usuario;
using Nutra.Seeder;
using Nutra.Services;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// ==================================================================
// DATA PROTECTION - Persistência de chaves para cookies sobreviverem restarts
// ==================================================================
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
    .SetApplicationName("nutra-api");

var myNextAppPolicy = "_myNextAppPolicy";

var authSettings = builder.Configuration.GetSection("Authentication");
var authority = authSettings["Authority"];
var clientId = authSettings["ClientId"];
var clientSecret = authSettings["ClientSecret"];

var connectionString = builder.Configuration
    ["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContextFactory<AlimentosContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AlimentosContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Nutra.Identity";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;

    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.ConfigureExternalCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = "OpenIdConnect";
})
.AddOpenIdConnect("OpenIdConnect", options =>
{
    options.Authority = authority;
    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.ResponseType = "code";
    options.SignInScheme = IdentityConstants.ApplicationScheme;

    // CallbackPath é relativo ao PathBase - NÃO incluir /nutra-api aqui
    // PathBase(/nutra-api) + CallbackPath(/signin-oidc) = /nutra-api/signin-oidc
    options.CallbackPath = "/signin-oidc";
    options.SignedOutCallbackPath = "/signout-callback-oidc";

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    // Configura��es para Desenvolvimento Local (Ignorar erro de SSL)
    options.RequireHttpsMetadata = false;
    options.BackchannelHttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role",
        ValidateIssuer = true,
        ValidIssuer = authority
    };

    // Escopos que vamos pedir ao Autenticador
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("offline_access");

    options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();

            var userIdExternal = context.Principal.FindFirst("sub")?.Value;
            var userEmail = context.Principal.FindFirst("email")?.Value;
            var userName = context.Principal.FindFirst("name")?.Value ?? userEmail;

            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = await userManager.FindByEmailAsync(userEmail);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        NomeCompleto = userName,
                        CPF = "",
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    await userManager.CreateAsync(user);
                }

                var principal = await signInManager.CreateUserPrincipalAsync(user);
                context.Principal = principal;
            }
        },

        OnRedirectToIdentityProvider = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api") &&
               !context.Request.Path.StartsWithSegments("/api/Auth/login"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.HandleResponse();
            }
            return Task.CompletedTask;
        },
        
        // Tratamento de erros de autenticação (ex: código já usado, refresh da página de callback)
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Falha na autenticação OIDC: {Error}", context.Exception.Message);
            
            // Redireciona para a página inicial com mensagem de erro
            // Isso evita o 502 quando o usuário atualiza a página de callback
            context.HandleResponse();
            context.Response.Redirect("/?auth_error=session_expired");
            return Task.CompletedTask;
        },
        
        // Tratamento de erros remotos (ex: invalid_grant)
        OnRemoteFailure = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Falha remota na autenticação: {Error}", context.Failure?.Message);
            
            context.HandleResponse();
            context.Response.Redirect("/?auth_error=remote_failure");
            return Task.CompletedTask;
        }
    };
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
            .AllowAnyMethod()
            .AllowCredentials());
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
// PATH BASE (OBRIGATÓRIO - app está montado em /nutra-api/)
// ==================================================================
app.UsePathBase("/nutra-api");

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
