using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Models;
using Nutra.Models.Alimentos;

/// <summary>
/// Busca paginada nas tabelas de alimentos.
/// <para>
/// Consulta o contexto direto, sem camada de service — logo é o controller que
/// monta o <see cref="RetornoPadrao{T}"/>. A semântica de status foi preservada
/// (404 quando a busca não retorna nada); só o shape do corpo mudou, para o
/// client ter um único formato em toda a API.
/// </para>
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AlimentosController : ControllerBase
{
    private readonly AlimentosContext _context;

    public AlimentosController(AlimentosContext context)
    {
        _context = context;
    }

    [HttpGet("fabricante/alimento/{alimento_fabricante}")]
    public async Task<IActionResult> BuscarAlimentosFabricantesPorNome(
        string alimento_fabricante,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Fabricantes
            .Where(a => a.Produto != null && a.Produto.Contains(alimento_fabricante));

        return await Paginar(alimento_fabricante, query, pageNumber, pageSize);
    }

    [HttpGet("fastfood/alimento/{nome_fastfood}")]
    public async Task<IActionResult> BuscarAlimentosFastFoodPorNome(
        string nome_fastfood,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.FastFoods
            .Where(a => a.Produto != null && a.Produto.ToLower().Contains(nome_fastfood));

        return await Paginar(nome_fastfood, query, pageNumber, pageSize);
    }

    [HttpGet("tbca/alimento/{nome_tbca}")]
    public async Task<IActionResult> BuscarAlimentosTbcaPorNome(
        string nome_tbca,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(nome_tbca))
            return Retornar(RetornoPadrao<PaginatedResultDto<Tbca>>.Invalido("O termo de busca não pode ser vazio."));

        var query = _context.Tbcas.AsQueryable();

        // TBCA busca por todas as palavras do termo (AND), não pela string inteira.
        foreach (var palavra in nome_tbca.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            var termo = palavra;
            query = query.Where(a => a.Nome != null && a.Nome.ToLower().Contains(termo));
        }

        return await Paginar(nome_tbca, query, pageNumber, pageSize);
    }

    [HttpGet("genericos/alimento/{nome_generico}")]
    public async Task<IActionResult> BuscarAlimentosGenericosPorNome(
        string nome_generico,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Genericos
            .Where(a => a.Produto != null && a.Produto.ToLower().Contains(nome_generico.ToLower()));

        return await Paginar(nome_generico, query, pageNumber, pageSize);
    }

    /// <summary>
    /// Pagina a consulta e envelopa o resultado. Extraído porque os 4 endpoints
    /// repetiam o mesmo bloco palavra por palavra.
    /// </summary>
    private async Task<IActionResult> Paginar<T>(
        string termo, IQueryable<T> query, int pageNumber, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return Retornar(RetornoPadrao<PaginatedResultDto<T>>.Invalido("O termo de busca não pode ser vazio."));

        var totalCount = await query.CountAsync();

        if (totalCount == 0)
            return Retornar(RetornoPadrao<PaginatedResultDto<T>>.NaoEncontrado(
                "Nenhum alimento encontrado com os termos informados."));

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var resultado = new PaginatedResultDto<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Items = items
        };

        return Retornar(RetornoPadrao<PaginatedResultDto<T>>.Ok(resultado));
    }

    private IActionResult Retornar(RetornoPadrao retorno) =>
        StatusCode(retorno.StatusCode, retorno);
}
