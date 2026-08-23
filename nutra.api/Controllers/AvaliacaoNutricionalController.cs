using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Interfaces;
using Nutra.Models.Dtos;
using System.Security.Claims;

namespace Nutra.Controllers;

/// <summary>
/// Controller para Avaliação Nutricional — Antropometria, Cálculos Automáticos e Fotos de Progresso.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AvaliacaoNutricionalController : ControllerBase
{
    private readonly IAvaliacaoNutricional _avaliacaoService;

    public AvaliacaoNutricionalController(IAvaliacaoNutricional avaliacaoService)
    {
        _avaliacaoService = avaliacaoService;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

    // =====================================================================
    //  AVALIAÇÕES — PACIENTE (auto-avaliação)
    // =====================================================================

    /// <summary>
    /// Registra uma nova avaliação antropométrica completa.
    /// Cálculos automáticos: IMC, RCQ, TMB (3 fórmulas), GET, %gordura, peso ideal, macros.
    /// </summary>
    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarAvaliacao([FromBody] AvaliacaoAntropometricaDto dto)
    {
        var retorno = await _avaliacaoService.RegistrarAvaliacaoAsync(GetUserId(), dto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Obtém avaliação completa por Id.
    /// </summary>
    [HttpGet("{avaliacaoId:int}")]
    public async Task<IActionResult> ObterAvaliacao(int avaliacaoId)
    {
        var retorno = await _avaliacaoService.ObterAvaliacaoPorIdAsync(GetUserId(), avaliacaoId);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Lista todas as avaliações do paciente logado (resumo).
    /// </summary>
    [HttpGet("minhas-avaliacoes")]
    public async Task<IActionResult> ListarMinhasAvaliacoes()
    {
        var retorno = await _avaliacaoService.ListarAvaliacoesAsync(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Compara duas avaliações e retorna evolução.
    /// </summary>
    [HttpGet("comparar/{anteriorId:int}/{atualId:int}")]
    public async Task<IActionResult> CompararAvaliacoes(int anteriorId, int atualId)
    {
        var retorno = await _avaliacaoService.CompararAvaliacoesAsync(GetUserId(), anteriorId, atualId);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Exclui uma avaliação.
    /// </summary>
    [HttpDelete("{avaliacaoId:int}")]
    public async Task<IActionResult> ExcluirAvaliacao(int avaliacaoId)
    {
        var retorno = await _avaliacaoService.ExcluirAvaliacaoAsync(GetUserId(), avaliacaoId);
        return StatusCode(retorno.StatusCode, retorno);
    }

    // =====================================================================
    //  AVALIAÇÕES — NUTRICIONISTA (em nome do paciente)
    // =====================================================================

    /// <summary>
    /// Nutricionista registra avaliação de um paciente vinculado.
    /// </summary>
    [HttpPost("paciente/{pacienteUserId}/registrar")]
    [Authorize(Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> RegistrarAvaliacaoPorProfissional(
        string pacienteUserId, [FromBody] AvaliacaoAntropometricaDto dto)
    {
        var retorno = await _avaliacaoService.RegistrarAvaliacaoPorProfissionalAsync(
            GetUserId(), pacienteUserId, dto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Nutricionista lista avaliações de um paciente vinculado.
    /// </summary>
    [HttpGet("paciente/{pacienteUserId}/avaliacoes")]
    [Authorize(Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> ListarAvaliacoesDoPaciente(string pacienteUserId)
    {
        var retorno = await _avaliacaoService.ListarAvaliacoesDoPacienteAsync(GetUserId(), pacienteUserId);
        return StatusCode(retorno.StatusCode, retorno);
    }

    // =====================================================================
    //  FOTOS DE PROGRESSO
    // =====================================================================

    /// <summary>
    /// Adiciona fotos de progresso a uma avaliação existente.
    /// </summary>
    [HttpPost("{avaliacaoId:int}/fotos")]
    public async Task<IActionResult> AdicionarFotos(int avaliacaoId, [FromBody] List<FotoProgressoDto> fotos)
    {
        var retorno = await _avaliacaoService.AdicionarFotosAsync(GetUserId(), avaliacaoId, fotos);
        return StatusCode(retorno.StatusCode, retorno);
    }

    /// <summary>
    /// Remove uma foto de progresso.
    /// </summary>
    [HttpDelete("fotos/{fotoId:int}")]
    public async Task<IActionResult> RemoverFoto(int fotoId)
    {
        var retorno = await _avaliacaoService.RemoverFotoAsync(GetUserId(), fotoId);
        return StatusCode(retorno.StatusCode, retorno);
    }
}
