using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Interfaces;
using Nutra.Models.Dtos;
using System.Security.Claims;

namespace Nutra.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DiarioAlimentarController : ControllerBase
{
    private readonly IDiarioAlimentar _diarioService;

    public DiarioAlimentarController(IDiarioAlimentar diarioService)
    {
        _diarioService = diarioService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // ============================================================
    // Registro de consumo
    // ============================================================

    /// <summary>
    /// Registra o consumo de um alimento no diário.
    /// </summary>
    [HttpPost("consumo")]
    public async Task<IActionResult> RegistrarConsumo([FromBody] RegistroConsumoDto dto)
    {
        try
        {
            var resultado = await _diarioService.RegistrarConsumoAsync(GetUserId(), dto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Registra o consumo de múltiplos alimentos de uma vez (lote).
    /// </summary>
    [HttpPost("consumo/lote")]
    public async Task<IActionResult> RegistrarConsumoLote([FromBody] RegistroConsumoLoteDto dto)
    {
        try
        {
            var resultado = await _diarioService.RegistrarConsumoLoteAsync(GetUserId(), dto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Exclui um registro de consumo do diário.
    /// </summary>
    [HttpDelete("consumo/{registroId:long}")]
    public async Task<IActionResult> ExcluirRegistro(long registroId)
    {
        var resultado = await _diarioService.ExcluirRegistroAsync(GetUserId(), registroId);
        return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
    }

    // ============================================================
    // Fotos de refeição
    // ============================================================

    /// <summary>
    /// Adiciona uma foto de refeição ao diário.
    /// </summary>
    [HttpPost("fotos")]
    public async Task<IActionResult> AdicionarFoto([FromBody] FotoRefeicaoDto dto)
    {
        var resultado = await _diarioService.AdicionarFotoRefeicaoAsync(GetUserId(), dto);
        return Ok(resultado);
    }

    /// <summary>
    /// Remove uma foto de refeição do diário.
    /// </summary>
    [HttpDelete("fotos/{fotoId:int}")]
    public async Task<IActionResult> RemoverFoto(int fotoId)
    {
        var resultado = await _diarioService.RemoverFotoRefeicaoAsync(GetUserId(), fotoId);
        return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
    }

    /// <summary>
    /// Lista fotos de refeição de um dia específico.
    /// </summary>
    [HttpGet("fotos")]
    public async Task<IActionResult> ListarFotosDoDia([FromQuery] DateTime? data = null)
    {
        var resultado = await _diarioService.ListarFotosDoDiaAsync(
            GetUserId(), data ?? DateTime.UtcNow);
        return Ok(resultado);
    }

    // ============================================================
    // Diário diário (planejado vs consumido)
    // ============================================================

    /// <summary>
    /// Obtém o diário completo de um dia, com comparação planejado vs consumido.
    /// </summary>
    [HttpGet("dia")]
    public async Task<IActionResult> ObterDiarioDoDia([FromQuery] DateTime? data = null)
    {
        var resultado = await _diarioService.ObterDiarioDoDiaAsync(GetUserId(), data);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtém o diário de um período (múltiplos dias).
    /// </summary>
    [HttpGet("periodo")]
    public async Task<IActionResult> ObterDiarioPeriodo(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim)
    {
        if (dataFim < dataInicio)
            return BadRequest(new { sucesso = false, mensagem = "Data fim deve ser posterior à data início." });

        if ((dataFim - dataInicio).TotalDays > 90)
            return BadRequest(new { sucesso = false, mensagem = "Período máximo de 90 dias." });

        var resultado = await _diarioService.ObterDiarioPorPeriodoAsync(GetUserId(), dataInicio, dataFim);
        return Ok(resultado);
    }

    // ============================================================
    // Relatório de aderência
    // ============================================================

    /// <summary>
    /// Gera relatório de aderência ao plano alimentar do próprio usuário.
    /// </summary>
    [HttpGet("relatorio-adesao")]
    public async Task<IActionResult> RelatorioAdesao(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim)
    {
        if (dataFim < dataInicio)
            return BadRequest(new { sucesso = false, mensagem = "Data fim deve ser posterior à data início." });

        var resultado = await _diarioService.GerarRelatorioAdesaoAsync(GetUserId(), dataInicio, dataFim);
        return Ok(resultado);
    }

    /// <summary>
    /// Gera relatório de aderência de um paciente (profissional com vínculo ativo).
    /// </summary>
    [HttpGet("relatorio-adesao/paciente/{pacienteUserId}")]
    public async Task<IActionResult> RelatorioAdesaoPaciente(
        string pacienteUserId,
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim)
    {
        try
        {
            if (dataFim < dataInicio)
                return BadRequest(new { sucesso = false, mensagem = "Data fim deve ser posterior à data início." });

            var resultado = await _diarioService.GerarRelatorioAdesaoPacienteAsync(
                GetUserId(), pacienteUserId, dataInicio, dataFim);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }
}
