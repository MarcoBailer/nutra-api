using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Enum;
using Nutra.Interfaces;
using System.Security.Claims;

namespace Nutra.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RefeicaoController : ControllerBase
{
    private readonly IRefeicao _refeicao;

    public RefeicaoController(IRefeicao refeicao)
    {
        _refeicao = refeicao;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

    [HttpPost("cadastrar-refeicao")]
    public async Task<IActionResult> PostConsumoAsync(int alimentoId, ETipoTabela tabela, double quantidadeIngeridaG, ETipoRefeicao nomeRefeicao)
    {
        var retorno = await _refeicao.RegistrarConsumoAsync(
            GetUserId(), alimentoId, tabela, quantidadeIngeridaG, nomeRefeicao);
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpGet("status-diario")]
    public async Task<IActionResult> GetStatusDiarioAsync()
    {
        var retorno = await _refeicao.ObterStatusDiario(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }
}
