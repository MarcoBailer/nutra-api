using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Enum;
using Nutra.Interfaces;

namespace Nutra.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BuscaController : ControllerBase
{
    private readonly IBusca _buscaService;

    public BuscaController(IBusca busca)
    {
        _buscaService = busca;
    }

    [HttpGet("BuscarTudo/{termo}")]
    public async Task<ActionResult> BuscarTudo(string termo)
    {
        if (string.IsNullOrWhiteSpace(termo) || termo.Length < 3)
            return BadRequest("Digite pelo menos 3 caracteres.");

        termo = termo.ToLower();

        var resultadoFinal = await _buscaService.BuscaAlimentoAsync(termo);

        return Ok(resultadoFinal);
    }

    [HttpGet("BuscarPorId/{id}/{tabela}")]
    public async Task<ActionResult> BuscarPorId(int id, ETipoTabela tabela)
    {
        var alimento = await _buscaService.BuscaAlimentoPorIdAsync(id, tabela);
        if (alimento == null)
            return NotFound("Alimento não encontrado.");
        return Ok(alimento);
    }
}
