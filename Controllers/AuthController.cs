using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Nutra.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var frontendUrl = _configuration["AppSettings:BaseUrlFront"] ?? "http://localhost:3000";
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = $"{frontendUrl}/pipboy"
        }, "OpenIdConnect");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        var frontendUrl = _configuration["AppSettings:BaseUrlFront"] ?? "http://localhost:3000";
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = $"{frontendUrl}/"
        }, "Cookies", "OpenIdConnect");
    }
}
