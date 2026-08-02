using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Interfaces;
using Nutra.Models.Dtos;
using System.Security.Claims;

namespace Nutra.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IApplicationUserService _applicationUserService;
    private readonly IAccounts _accounts;
    private readonly INutricionista _nutricionista;

    public AccountsController(
        IApplicationUserService applicationUserService,
        IAccounts accounts,
        INutricionista nutricionista)
    {
        _applicationUserService = applicationUserService;
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
        var userId = GetUserId();
        var user = await _applicationUserService.FindByIdAsync(userId);

        if (user == null)
        {
            return Unauthorized("Usuário autenticado não possui projeção local na NutraApi.");
        }

        return Ok(new
        {
            user.Id,
            user.NomeCompleto,
            user.Email,
            user.CPF,
            Roles = User.FindAll("role").Select(c => c.Value).ToArray(),
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
