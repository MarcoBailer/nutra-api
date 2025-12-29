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

    [HttpPost("cadastrar-refeicao")]
    public async Task<IActionResult> PostConsumoAsync(int alimentoId, ETipoTabela tabela, double quantidadeIngeridaG, ETipoRefeicao nomeRefeicao)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Token inválido: ID do usuário não encontrado.");
        }
        var result = await _refeicao.RegistrarConsumoAsync(userId, alimentoId, tabela, quantidadeIngeridaG, nomeRefeicao);
        return Ok(result);
    }

    [HttpGet("status-diario")]
    public async Task<IActionResult> GetStatusDiarioAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Token inválido: ID do usuário não encontrado.");
        }
        var status = await _refeicao.ObterStatusDiario(userId);
        return Ok(status);
    }
}
