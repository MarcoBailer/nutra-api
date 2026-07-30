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
        try
        {
            var userId = GetUserId();
            var resultado = await _avaliacaoService.RegistrarAvaliacaoAsync(userId, dto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Obtém avaliação completa por Id.
    /// </summary>
    [HttpGet("{avaliacaoId:int}")]
    public async Task<IActionResult> ObterAvaliacao(int avaliacaoId)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _avaliacaoService.ObterAvaliacaoPorIdAsync(userId, avaliacaoId);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Sucesso = false, Mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Lista todas as avaliações do paciente logado (resumo).
    /// </summary>
    [HttpGet("minhas-avaliacoes")]
    public async Task<IActionResult> ListarMinhasAvaliacoes()
    {
        try
        {
            var userId = GetUserId();
            var lista = await _avaliacaoService.ListarAvaliacoesAsync(userId);
            return Ok(lista);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Compara duas avaliações e retorna evolução.
    /// </summary>
    [HttpGet("comparar/{anteriorId:int}/{atualId:int}")]
    public async Task<IActionResult> CompararAvaliacoes(int anteriorId, int atualId)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _avaliacaoService.CompararAvaliacoesAsync(userId, anteriorId, atualId);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Sucesso = false, Mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Exclui uma avaliação.
    /// </summary>
    [HttpDelete("{avaliacaoId:int}")]
    public async Task<IActionResult> ExcluirAvaliacao(int avaliacaoId)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _avaliacaoService.ExcluirAvaliacaoAsync(userId, avaliacaoId);
            return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
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
        try
        {
            var profissionalUserId = GetUserId();
            var resultado = await _avaliacaoService.RegistrarAvaliacaoPorProfissionalAsync(
                profissionalUserId, pacienteUserId, dto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Nutricionista lista avaliações de um paciente vinculado.
    /// </summary>
    [HttpGet("paciente/{pacienteUserId}/avaliacoes")]
    [Authorize(Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> ListarAvaliacoesDoPaciente(string pacienteUserId)
    {
        try
        {
            var profissionalUserId = GetUserId();
            var lista = await _avaliacaoService.ListarAvaliacoesDoPacienteAsync(profissionalUserId, pacienteUserId);
            return Ok(lista);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
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
        try
        {
            var userId = GetUserId();
            var resultado = await _avaliacaoService.AdicionarFotosAsync(userId, avaliacaoId, fotos);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Remove uma foto de progresso.
    /// </summary>
    [HttpDelete("fotos/{fotoId:int}")]
    public async Task<IActionResult> RemoverFoto(int fotoId)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _avaliacaoService.RemoverFotoAsync(userId, fotoId);
            return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }
}
