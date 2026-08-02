using Nutra.Enum;
using Nutra.Models;
using Nutra.Models.Dtos;

namespace Nutra.Interfaces;

public interface IRefeicao
{
    Task<RetornoPadrao> RegistrarConsumoAsync(string userId, int alimentoId, ETipoTabela tabela, double quantidadeIngeridaG, ETipoRefeicao nomeRefeicao);
    Task<RetornoPadrao<StatusDiarioDto>> ObterStatusDiario(string userId);
}
