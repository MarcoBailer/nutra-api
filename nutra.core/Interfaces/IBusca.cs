using Nutra.Enum;
using Nutra.Models.Dtos;

namespace Nutra.Interfaces;

public interface IBusca
{
    Task<List<AlimentoResumoDto>> BuscaAlimentoAsync(string termo);
    /// <summary>Retorna null quando o alimento não existe na tabela informada.</summary>
    Task<AlimentoResumoDto?> BuscaAlimentoPorIdAsync(int id, ETipoTabela tabela);

    /// <summary>
    /// Busca paginada restrita a uma tabela. Casa apenas os registros que contêm
    /// <b>todas</b> as palavras do termo, sem distinção de maiúsculas.
    /// <para>
    /// Página vazia com <c>TotalCount = 0</c> significa nada encontrado — quem
    /// traduz isso em status HTTP é o controller.
    /// </para>
    /// </summary>
    Task<PaginatedResultDto<AlimentoResumoDto>> BuscaAlimentoPaginadoAsync(
        string termo, ETipoTabela tabela, int pageNumber, int pageSize);
}
