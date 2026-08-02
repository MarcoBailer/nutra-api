using Nutra.Enum;
using Nutra.Models.Dtos;

namespace Nutra.Interfaces;

public interface IBusca
{
    Task<List<AlimentoResumoDto>> BuscaAlimentoAsync(string termo);
    /// <summary>Retorna null quando o alimento não existe na tabela informada.</summary>
    Task<AlimentoResumoDto?> BuscaAlimentoPorIdAsync(int id, ETipoTabela tabela);
}
