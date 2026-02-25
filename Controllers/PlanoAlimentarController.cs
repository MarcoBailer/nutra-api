using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Interfaces;
using Nutra.Models.Dtos;
using System.Security.Claims;

namespace Nutra.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PlanoAlimentarController : ControllerBase
{
    private readonly IPlanoAlimentar _planoService;

    public PlanoAlimentarController(IPlanoAlimentar planoService)
    {
        _planoService = planoService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // ============================================================
    // CRUD Plano
    // ============================================================

    /// <summary>
    /// Cria um novo plano alimentar para o usuário autenticado.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarPlano([FromBody] CriarPlanoAlimentarDto dto)
    {
        try
        {
            var resultado = await _planoService.CriarPlanoAsync(GetUserId(), dto);
            return CreatedAtAction(nameof(ObterPlano), new { planoId = resultado.Id }, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Cria um plano alimentar para um paciente vinculado (profissional).
    /// </summary>
    [HttpPost("profissional")]
    public async Task<IActionResult> CriarPlanoProfissional([FromBody] CriarPlanoProfissionalDto dto)
    {
        try
        {
            var resultado = await _planoService.CriarPlanoPorProfissionalAsync(GetUserId(), dto);
            return CreatedAtAction(nameof(ObterPlano), new { planoId = resultado.Id }, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Obtém um plano alimentar específico pelo ID.
    /// </summary>
    [HttpGet("{planoId:int}")]
    public async Task<IActionResult> ObterPlano(int planoId)
    {
        try
        {
            var resultado = await _planoService.ObterPlanoAsync(GetUserId(), planoId);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Obtém o plano alimentar ativo do usuário.
    /// </summary>
    [HttpGet("ativo")]
    public async Task<IActionResult> ObterPlanoAtivo()
    {
        var resultado = await _planoService.ObterPlanoAtivoAsync(GetUserId());
        if (resultado == null)
            return NotFound(new { sucesso = false, mensagem = "Nenhum plano alimentar ativo encontrado." });
        return Ok(resultado);
    }

    /// <summary>
    /// Lista todos os planos alimentares do usuário.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListarPlanos()
    {
        try
        {
            var resultado = await _planoService.ListarPlanosAsync(GetUserId());
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza informações gerais de um plano alimentar.
    /// </summary>
    [HttpPut("{planoId:int}")]
    public async Task<IActionResult> AtualizarPlano(int planoId, [FromBody] AtualizarPlanoAlimentarDto dto)
    {
        try
        {
            var resultado = await _planoService.AtualizarPlanoAsync(GetUserId(), planoId, dto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Exclui um plano alimentar.
    /// </summary>
    [HttpDelete("{planoId:int}")]
    public async Task<IActionResult> ExcluirPlano(int planoId)
    {
        var resultado = await _planoService.ExcluirPlanoAsync(GetUserId(), planoId);
        return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
    }

    /// <summary>
    /// Ativa um plano alimentar (desativa o anterior automaticamente).
    /// </summary>
    [HttpPost("{planoId:int}/ativar")]
    public async Task<IActionResult> AtivarPlano(int planoId)
    {
        var resultado = await _planoService.AtivarPlanoAsync(GetUserId(), planoId);
        return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
    }

    // ============================================================
    // Refeições
    // ============================================================

    /// <summary>
    /// Adiciona uma nova refeição a um plano alimentar.
    /// </summary>
    [HttpPost("{planoId:int}/refeicoes")]
    public async Task<IActionResult> AdicionarRefeicao(int planoId, [FromBody] AdicionarRefeicaoDto dto)
    {
        try
        {
            var resultado = await _planoService.AdicionarRefeicaoAsync(GetUserId(), planoId, dto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Remove uma refeição de um plano alimentar.
    /// </summary>
    [HttpDelete("refeicoes/{refeicaoId:int}")]
    public async Task<IActionResult> RemoverRefeicao(int refeicaoId)
    {
        var resultado = await _planoService.RemoverRefeicaoAsync(GetUserId(), refeicaoId);
        return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
    }

    // ============================================================
    // Itens
    // ============================================================

    /// <summary>
    /// Adiciona um item alimentar a uma refeição do plano.
    /// </summary>
    [HttpPost("refeicoes/{refeicaoId:int}/itens")]
    public async Task<IActionResult> AdicionarItem(int refeicaoId, [FromBody] AdicionarItemDto dto)
    {
        try
        {
            var resultado = await _planoService.AdicionarItemAsync(GetUserId(), refeicaoId, dto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Remove um item de uma refeição do plano.
    /// </summary>
    [HttpDelete("itens/{itemId:int}")]
    public async Task<IActionResult> RemoverItem(int itemId)
    {
        var resultado = await _planoService.RemoverItemAsync(GetUserId(), itemId);
        return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
    }

    // ============================================================
    // Substituições
    // ============================================================

    /// <summary>
    /// Adiciona uma substituição equivalente a um item do plano.
    /// </summary>
    [HttpPost("itens/{itemId:int}/substituicoes")]
    public async Task<IActionResult> AdicionarSubstituicao(int itemId, [FromBody] AdicionarSubstituicaoDto dto)
    {
        var resultado = await _planoService.AdicionarSubstituicaoAsync(GetUserId(), itemId, dto);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    /// <summary>
    /// Remove uma substituição equivalente.
    /// </summary>
    [HttpDelete("substituicoes/{substituicaoId:int}")]
    public async Task<IActionResult> RemoverSubstituicao(int substituicaoId)
    {
        var resultado = await _planoService.RemoverSubstituicaoAsync(GetUserId(), substituicaoId);
        return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
    }

    // ============================================================
    // Modelos de Dieta (Templates)
    // ============================================================

    /// <summary>
    /// Cria um novo modelo/template de dieta (profissional).
    /// </summary>
    [HttpPost("modelos")]
    public async Task<IActionResult> CriarModeloDieta([FromBody] CriarModeloDietaDto dto)
    {
        try
        {
            var resultado = await _planoService.CriarModeloDietaAsync(GetUserId(), dto);
            return CreatedAtAction(nameof(ObterModeloDieta), new { modeloId = resultado.Id }, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Lista modelos de dieta disponíveis (públicos + do profissional).
    /// </summary>
    [HttpGet("modelos")]
    public async Task<IActionResult> ListarModelos()
    {
        var resultado = await _planoService.ListarModelosDietaAsync(GetUserId());
        return Ok(resultado);
    }

    /// <summary>
    /// Obtém detalhes de um modelo de dieta.
    /// </summary>
    [HttpGet("modelos/{modeloId:int}")]
    public async Task<IActionResult> ObterModeloDieta(int modeloId)
    {
        try
        {
            var resultado = await _planoService.ObterModeloDietaAsync(modeloId);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { sucesso = false, mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Exclui (soft delete) um modelo de dieta criado pelo profissional.
    /// </summary>
    [HttpDelete("modelos/{modeloId:int}")]
    public async Task<IActionResult> ExcluirModeloDieta(int modeloId)
    {
        var resultado = await _planoService.ExcluirModeloDietaAsync(GetUserId(), modeloId);
        return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
    }

    /// <summary>
    /// Cria um plano alimentar a partir de um modelo de dieta, escalonado para as metas do usuário.
    /// </summary>
    [HttpPost("modelos/{modeloId:int}/criar-plano")]
    public async Task<IActionResult> CriarPlanoAPartirDeModelo(
        int modeloId,
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime? dataFim = null)
    {
        try
        {
            var resultado = await _planoService.CriarPlanoAPartirDeModeloAsync(GetUserId(), modeloId, dataInicio, dataFim);
            return CreatedAtAction(nameof(ObterPlano), new { planoId = resultado.Id }, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }
}
