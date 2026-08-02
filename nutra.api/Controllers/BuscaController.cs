using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;

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

    /// <summary>
    /// IBusca não retorna envelope (é consumido internamente por outros services).
    /// É o controller que envelopa, para o client ver o mesmo shape de sempre.
    /// </summary>
    [HttpGet("BuscarTudo/{termo}")]
    public async Task<IActionResult> BuscarTudo(string termo)
    {
        if (string.IsNullOrWhiteSpace(termo) || termo.Length < 3)
        {
            var invalido = RetornoPadrao<List<AlimentoResumoDto>>.Invalido("Digite pelo menos 3 caracteres.");
            return StatusCode(invalido.StatusCode, invalido);
        }

        var alimentos = await _buscaService.BuscaAlimentoAsync(termo.ToLower());
        var retorno = RetornoPadrao<List<AlimentoResumoDto>>.Ok(alimentos);
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpGet("BuscarPorId/{id}/{tabela}")]
    public async Task<IActionResult> BuscarPorId(int id, ETipoTabela tabela)
    {
        var alimento = await _buscaService.BuscaAlimentoPorIdAsync(id, tabela);

        var retorno = alimento == null
            ? RetornoPadrao<AlimentoResumoDto>.NaoEncontrado("Alimento não encontrado.")
            : RetornoPadrao<AlimentoResumoDto>.Ok(alimento);

        return StatusCode(retorno.StatusCode, retorno);
    }
}
