using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nutra.Helper;

namespace Nutra.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AuthLogger _authLogger;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, AuthLogger authLogger, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _authLogger = authLogger;
        _logger = logger;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var frontendUrl = _configuration["AppSettings:BaseUrlFront"] ?? "http://localhost:3000";
        var userId = User?.FindFirst("sub")?.Value ?? "ANONYMOUS";
        
        _authLogger.LogAuthStart(userId, "GET", "/api/Auth/login");
        _logger.LogInformation($"[NutraFoodApi] Auth/login iniciado. Frontend URL: {frontendUrl}");
        
        var redirectUri = $"{frontendUrl}/pipboy";
        _authLogger.LogAuthStep("LOGIN-CHALLENGE", $"Iniciando OpenIdConnect Challenge com redirectUri: {redirectUri}", userId);
        _logger.LogInformation($"[NutraFoodApi] Desafiando OpenIdConnect. RedirectUri: {redirectUri}");
        
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = redirectUri
        }, "OpenIdConnect");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        var frontendUrl = _configuration["AppSettings:BaseUrlFront"] ?? "http://localhost:3000";
        var userId = User?.FindFirst("sub")?.Value ?? "ANONYMOUS";
        
        _authLogger.LogAuthStart(userId, "GET", "/api/Auth/logout");
        _logger.LogInformation($"[NutraFoodApi] Auth/logout iniciado");
        
        var redirectUri = $"{frontendUrl}/";
        _authLogger.LogAuthStep("LOGOUT-SIGNOUT", $"Iniciando SignOut com redirectUri: {redirectUri}", userId);
        _logger.LogInformation($"[NutraFoodApi] Assinando saída. RedirectUri: {redirectUri}");
        
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = redirectUri
        }, "Cookies", "OpenIdConnect");
    }
}
