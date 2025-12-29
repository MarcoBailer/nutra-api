using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Nutra.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "http://localhost:3000/pipboy"
        }, "OpenIdConnect");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        // Desloga dos Cookies locais E avisa o Projeto A para deslogar lá também
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = "http://localhost:3000/"
        }, "Cookies", "OpenIdConnect");
    }
}
