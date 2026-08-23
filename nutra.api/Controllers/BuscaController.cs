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

    /// <summary>
    /// Busca paginada dentro de uma única tabela. Substitui os quatro endpoints
    /// de <c>api/Alimentos</c>, que só diferiam pela tabela — que já é
    /// <see cref="ETipoTabela"/>, o mesmo discriminador de <c>BuscarPorId</c>.
    /// <para>
    /// Semântica de status preservada: termo vazio é 400, zero resultados é 404.
    /// </para>
    /// </summary>
    [HttpGet("BuscarPorTabela/{tabela}/{termo}")]
    public async Task<IActionResult> BuscarPorTabela(
        ETipoTabela tabela,
        string termo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(termo))
        {
            var invalido = RetornoPadrao<PaginatedResultDto<AlimentoResumoDto>>
                .Invalido("O termo de busca não pode ser vazio.");
            return StatusCode(invalido.StatusCode, invalido);
        }

        // Sem o piso, Skip recebe valor negativo e estoura em 500. O teto evita
        // que o client peça a tabela inteira em uma requisição.
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
        {
            var invalido = RetornoPadrao<PaginatedResultDto<AlimentoResumoDto>>
                .Invalido("pageNumber deve ser >= 1 e pageSize deve estar entre 1 e 100.");
            return StatusCode(invalido.StatusCode, invalido);
        }

        var pagina = await _buscaService.BuscaAlimentoPaginadoAsync(termo, tabela, pageNumber, pageSize);

        var retorno = pagina.TotalCount == 0
            ? RetornoPadrao<PaginatedResultDto<AlimentoResumoDto>>
                .NaoEncontrado("Nenhum alimento encontrado com os termos informados.")
            : RetornoPadrao<PaginatedResultDto<AlimentoResumoDto>>.Ok(pagina);

        return StatusCode(retorno.StatusCode, retorno);
    }
}
