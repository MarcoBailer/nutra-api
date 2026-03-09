using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nutra.Data;
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
    // SameSite=None permite cookies cross-site (necessário quando frontend e backend estão em portas diferentes)
    // Secure é obrigatório para SameSite=None
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
    // Usar Bearer como padrão para APIs que recebem tokens JWT via Authorization header
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = authority;
    options.RequireHttpsMetadata = false; // Dev only - mudar para true em produção

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = authority,
        ValidateAudience = false, // Desabilitado: token não inclui 'aud' claim
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
            
            // Log all claims for debugging
            if (context.Principal?.Identity is ClaimsIdentity identity)
            {
                logger.LogInformation($"[NutraFoodApi JWT] Claims no token:");
                foreach (var claim in identity.Claims)
                {
                    logger.LogInformation($"  - {claim.Type}: {claim.Value}");
                }
                
                var userId = context.Principal?.FindFirst("sub")?.Value ?? 
                            context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                            context.Principal?.FindFirst("client_id")?.Value ?? 
                            "UNKNOWN";
                
                logger.LogInformation($"[NutraFoodApi JWT] IsAuthenticated: {context.Principal?.Identity?.IsAuthenticated}");
                logger.LogInformation($"[NutraFoodApi JWT] AuthenticationType: {context.Principal?.Identity?.AuthenticationType}");
                logger.LogInformation($"[NutraFoodApi JWT] UserId (web-auth): {userId}");
                
                authLogger.LogOpenIdEvent("JWT-TOKEN-VALIDATED", $"Bearer token validado", userId);

                // Resolve the local NutraApi user ID from the email claim and replace
                // the web-auth sub so all controllers receive the correct local GUID.
                var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value
                         ?? context.Principal?.FindFirst("email")?.Value;

                if (!string.IsNullOrEmpty(email))
                {
                    var userManager = context.HttpContext.RequestServices
                        .GetRequiredService<UserManager<ApplicationUser>>();
                    var localUser = await userManager.FindByEmailAsync(email);
                    if (localUser != null)
                    {
                        var existing = identity.FindFirst(ClaimTypes.NameIdentifier);
                        if (existing != null)
                            identity.RemoveClaim(existing);
                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, localUser.Id));
                        logger.LogInformation($"[NutraFoodApi JWT] Resolved local NutraApi userId: {localUser.Id}");
                    }
                }
            }
            else
            {
                logger.LogWarning("[NutraFoodApi JWT] Principal ou Identity é nulo!");
            }
        }
    };
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
            var authLogger = context.HttpContext.RequestServices.GetRequiredService<AuthLogger>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var userIdExternal = context.Principal.FindFirst("sub")?.Value ?? "UNKNOWN";
            
            authLogger.LogOpenIdEvent("TOKEN-VALIDATED", $"Token recebido do servidor OpenID", userIdExternal);
            logger.LogInformation($"[NutraFoodApi] Token validado. ExternalId: {userIdExternal}");
            
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();

            var userEmail = context.Principal.FindFirst("email")?.Value;
            var userName = context.Principal.FindFirst("name")?.Value ?? userEmail;

            authLogger.LogAuthStep("EMAIL-EXTRACTION", $"Email: {userEmail}, Nome: {userName}", userIdExternal);
            logger.LogInformation($"[NutraFoodApi] Email extraído: {userEmail}, Nome: {userName}");

            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = await userManager.FindByEmailAsync(userEmail);

                if (user == null)
                {
                    authLogger.LogAuthStep("USER-CREATE", $"Usuário não encontrado. Criando novo usuário para email: {userEmail}", userIdExternal);
                    logger.LogInformation($"[NutraFoodApi] Criando novo usuário para {userEmail}");
                    
                    user = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        NomeCompleto = userName,
                        CPF = "",
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    var createResult = await userManager.CreateAsync(user);
                    
                    authLogger.LogAuthStep("USER-CREATED", $"Status: {(createResult.Succeeded ? "Sucesso" : "Falha")}, Erros: {string.Join(", ", createResult.Errors.Select(e => e.Description))}", userIdExternal);
                    logger.LogInformation($"[NutraFoodApi] Usuário criado: {(createResult.Succeeded ? "Sucesso" : "Falha")}");
                }
                else
                {
                    authLogger.LogAuthStep("USER-FOUND", $"Usuário encontrado no banco de dados", userIdExternal);
                    logger.LogInformation($"[NutraFoodApi] Usuário encontrado: {userEmail}");
                }

                var principal = await signInManager.CreateUserPrincipalAsync(user);
                context.Principal = principal;
                
                authLogger.LogAuthStep("PRINCIPAL-CREATED", $"Principal criado para usuário {user.Email}", userIdExternal);
                logger.LogInformation($"[NutraFoodApi] Principal criado para {user.Email}");
            }
            else
            {
                authLogger.LogWarning("TOKEN-VALIDATION", "Email não encontrado no token", userIdExternal);
                logger.LogWarning($"[NutraFoodApi] Email não encontrado no token");
            }
        },

        OnRedirectToIdentityProvider = context =>
        {
            var authLogger = context.HttpContext.RequestServices.GetRequiredService<AuthLogger>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var userId = context.HttpContext.User?.FindFirst("sub")?.Value ?? "ANONYMOUS";
            
            var redirectUri = context.ProtocolMessage.RedirectUri;
            var clientId = context.ProtocolMessage.ClientId;
            var scope = context.ProtocolMessage.Scope;
            
            authLogger.LogAuthStep("REDIRECT-TO-PROVIDER", $"RedirectUri: {redirectUri}, ClientId: {clientId}, Scope: {scope}", userId);
            logger.LogInformation("[NutraFoodApi] Redirecionando para provedor OpenID. ClientId: {ClientId}, RedirectUri: {RedirectUri}", clientId, redirectUri);
            
            if (context.Request.Path.StartsWithSegments("/api") &&
               !context.Request.Path.StartsWithSegments("/api/Auth/login"))
            {
                authLogger.LogWarning("REDIRECT-BLOCKED", $"Redirecionamento bloqueado para path: {context.Request.Path}", userId);
                logger.LogWarning("[NutraFoodApi] Redirecionamento bloqueado para {Path}", context.Request.Path);
                
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.HandleResponse();
            }
            return Task.CompletedTask;
        },
        
        OnAuthenticationFailed = context =>
        {
            var authLogger = context.HttpContext.RequestServices.GetRequiredService<AuthLogger>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var userId = context.HttpContext.User?.FindFirst("sub")?.Value ?? "ANONYMOUS";
            
            authLogger.LogException("AUTH-FAILED-EVENT", context.Exception, userId);
            logger.LogError(context.Exception, $"[NutraFoodApi] Falha na autenticação OIDC: {context.Exception.Message}");
            
            context.HandleResponse();
            context.Response.Redirect("/?auth_error=session_expired");
            return Task.CompletedTask;
        },
        
        OnRemoteFailure = context =>
        {
            var authLogger = context.HttpContext.RequestServices.GetRequiredService<AuthLogger>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var userId = context.HttpContext.User?.FindFirst("sub")?.Value ?? "ANONYMOUS";
            
            authLogger.LogWarning("REMOTE-FAILURE", $"Erro remoto: {context.Failure?.Message}", userId);
            logger.LogError($"[NutraFoodApi] Falha remota: {context.Failure?.Message}");
            
            context.HandleResponse();
            context.Response.Redirect("/?auth_error=remote_failure");
            return Task.CompletedTask;
        }
    };
});


// AUTORIZAÇÃO: Configura política padrão que requer qualquer usuário autenticado
builder.Services.AddAuthorization(options =>
{
    // Política padrão: usuário deve estar autenticado via qualquer esquema
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("Bearer", "OpenIdConnect")
        .Build();
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
