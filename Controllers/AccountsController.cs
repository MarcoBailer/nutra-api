using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models.Dtos;
using Nutra.Models.Dtos.Registro;
using Nutra.Models.Usuario;
using System.Security.Claims;

namespace Nutra.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAccounts _accounts;
    private readonly INutricionista _nutricionista;

    public AccountsController(
        UserManager<ApplicationUser> userManager,
        IAccounts accounts,
        INutricionista nutricionista)
    {
        _userManager = userManager;
        _accounts = accounts;
        _nutricionista = nutricionista;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

    /// <summary>
    /// Retorna o perfil completo do usuário autenticado (dados pessoais + role).
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("name");

        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized("Token inválido: E-mail não encontrado nas claims.");
        }

        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                NomeCompleto = userName ?? "Usuário Novo",
                CPF = "",
                Role = ETipoRole.Paciente,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                return BadRequest("Erro ao sincronizar usuário no banco local.");
            }
        }

        return Ok(new
        {
            user.Id,
            user.NomeCompleto,
            user.Email,
            user.CPF,
            user.Role,
            user.DataNascimento,
            user.Telefone,
            user.FotoPerfilUrl,
            user.Ativo,
            user.CriadoEm,
            Endereco = new
            {
                user.Logradouro,
                user.Numero,
                user.Complemento,
                user.Bairro,
                user.Cidade,
                user.Estado,
                user.CEP
            }
        });
    }

    /// <summary>
    /// Atualiza dados pessoais do usuário: nome, CPF, endereço, telefone, etc.
    /// </summary>
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _accounts.AtualizarPerfilAsync(userId, model);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Desativa a conta do usuário (soft delete).
    /// </summary>
    [HttpPost("desativar")]
    public async Task<IActionResult> DesativarConta()
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _accounts.DesativarContaAsync(userId);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Reativa a conta do usuário.
    /// </summary>
    [HttpPost("reativar")]
    public async Task<IActionResult> ReativarConta()
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _accounts.ReativarContaAsync(userId);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Paciente responde ao convite de vínculo com nutricionista.
    /// </summary>
    [HttpPost("vinculos/{vinculoId}/responder")]
    public async Task<IActionResult> ResponderConvite(int vinculoId, [FromQuery] bool aceitar)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _nutricionista.ResponderConviteAsync(userId, vinculoId, aceitar);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Lista nutricionistas vinculados ao paciente.
    /// </summary>
    [HttpGet("meus-nutricionistas")]
    public async Task<IActionResult> ListarMeusNutricionistas()
    {
        try
        {
            var userId = GetUserId();
            var nutricionistas = await _nutricionista.ListarNutricionistasAsync(userId);
            return Ok(nutricionistas);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }
}
