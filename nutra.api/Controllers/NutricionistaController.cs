using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models.Dtos;
using System.Security.Claims;

namespace Nutra.Controllers;

/// <summary>
/// Controller para gestão de nutricionistas: perfil profissional, clínicas, pacientes e assinatura.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Nutricionista,Admin")]
public class NutricionistaController : ControllerBase
{
    private readonly INutricionista _nutricionista;

    public NutricionistaController(INutricionista nutricionista)
    {
        _nutricionista = nutricionista;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

    // ===================== CADASTRO / PERFIL =====================

    /// <summary>
    /// Cadastra um novo profissional nutricionista (pode ser chamado sem role se for auto-cadastro).
    /// </summary>
    [HttpPost("cadastro")]
    [AllowAnonymous]
    public async Task<IActionResult> Cadastrar([FromBody] CadastroNutricionistaDto dto)
    {
        var retorno = await _nutricionista.CadastrarNutricionistaAsync(dto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Obtém o perfil profissional do nutricionista logado.
    /// </summary>
    [HttpGet("perfil")]
    public async Task<IActionResult> ObterPerfil()
    {
        var retorno = await _nutricionista.ObterPerfilProfissionalAsync(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Atualiza dados do perfil profissional.
    /// </summary>
    [HttpPut("perfil")]
    public async Task<IActionResult> AtualizarPerfil([FromBody] UpdatePerfilProfissionalDto dto)
    {
        var retorno = await _nutricionista.AtualizarPerfilProfissionalAsync(GetUserId(), dto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    // ===================== CLÍNICAS =====================

    /// <summary>
    /// Lista clínicas ativas do nutricionista.
    /// </summary>
    [HttpGet("clinicas")]
    public async Task<IActionResult> ListarClinicas()
    {
        var retorno = await _nutricionista.ListarClinicasAsync(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Cria uma nova clínica.
    /// </summary>
    [HttpPost("clinicas")]
    public async Task<IActionResult> CriarClinica([FromBody] ClinicaDto dto)
    {
        var retorno = await _nutricionista.CriarClinicaAsync(GetUserId(), dto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Atualiza uma clínica existente.
    /// </summary>
    [HttpPut("clinicas/{clinicaId}")]
    public async Task<IActionResult> AtualizarClinica(int clinicaId, [FromBody] ClinicaDto dto)
    {
        var retorno = await _nutricionista.AtualizarClinicaAsync(GetUserId(), clinicaId, dto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Remove (soft delete) uma clínica.
    /// </summary>
    [HttpDelete("clinicas/{clinicaId}")]
    public async Task<IActionResult> RemoverClinica(int clinicaId)
    {
        var retorno = await _nutricionista.RemoverClinicaAsync(GetUserId(), clinicaId);
        return StatusCode(retorno.StatusCode, retorno);
    }

    // ===================== GESTÃO DE PACIENTES =====================

    /// <summary>
    /// Lista pacientes vinculados ao nutricionista.
    /// </summary>
    [HttpGet("pacientes")]
    public async Task<IActionResult> ListarPacientes()
    {
        var retorno = await _nutricionista.ListarPacientesAsync(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Envia convite de vínculo para um paciente (por e-mail).
    /// </summary>
    [HttpPost("pacientes/convite")]
    public async Task<IActionResult> EnviarConvite([FromBody] ConviteVinculoDto dto)
    {
        var retorno = await _nutricionista.EnviarConvitePacienteAsync(GetUserId(), dto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Encerra um vínculo com paciente.
    /// </summary>
    [HttpDelete("pacientes/vinculo/{vinculoId}")]
    public async Task<IActionResult> EncerrarVinculo(int vinculoId)
    {
        var retorno = await _nutricionista.EncerrarVinculoAsync(GetUserId(), vinculoId);
        return StatusCode(retorno.StatusCode, retorno);
    }

    // ===================== ASSINATURA =====================

    /// <summary>
    /// Atualiza o plano de assinatura do nutricionista.
    /// </summary>
    [HttpPut("assinatura/{novoPlano}")]
    public async Task<IActionResult> AtualizarPlano(EPlanoAssinatura novoPlano)
    {
        var retorno = await _nutricionista.AtualizarPlanoAsync(GetUserId(), novoPlano);
        return StatusCode(retorno.StatusCode, retorno);
    }
}
