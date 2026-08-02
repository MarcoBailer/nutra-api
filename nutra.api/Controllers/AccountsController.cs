using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Interfaces;
using Nutra.Models;
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
            var falha = RetornoPadrao<object>.NaoEncontrado(
                "Usuário autenticado não possui projeção local na NutraApi.");
            return StatusCode(falha.StatusCode, falha);
        }

        var dados = new
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
        };

        var retorno = RetornoPadrao<object>.Ok(dados);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Atualiza dados pessoais do usuário: nome, CPF, endereço, telefone, etc.
    /// </summary>
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
    {
        var retorno = await _accounts.AtualizarPerfilAsync(GetUserId(), model);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Desativa a conta do usuário (soft delete).
    /// </summary>
    [HttpPost("desativar")]
    public async Task<IActionResult> DesativarConta()
    {
        var retorno = await _accounts.DesativarContaAsync(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Reativa a conta do usuário.
    /// </summary>
    [HttpPost("reativar")]
    public async Task<IActionResult> ReativarConta()
    {
        var retorno = await _accounts.ReativarContaAsync(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Paciente responde ao convite de vínculo com nutricionista.
    /// </summary>
    [HttpPost("vinculos/{vinculoId}/responder")]
    public async Task<IActionResult> ResponderConvite(int vinculoId, [FromQuery] bool aceitar)
    {
        var retorno = await _nutricionista.ResponderConviteAsync(GetUserId(), vinculoId, aceitar);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Lista nutricionistas vinculados ao paciente.
    /// </summary>
    [HttpGet("meus-nutricionistas")]
    public async Task<IActionResult> ListarMeusNutricionistas()
    {
        var retorno = await _nutricionista.ListarNutricionistasAsync(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }
}
