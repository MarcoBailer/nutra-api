using Nutra.Enum;
using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IPreferenciaAlimentarRepository : IBaseRepository<PreferenciaAlimentar>
{
    Task<PreferenciaAlimentar?> ObterPorPerfilEAlimentoAsync(
        int perfilNutricionalId, int alimentoId, ETipoTabela tabela);

    Task<PreferenciaAlimentar?> ObterPorIdEUsuarioAsync(int preferenciaId, string userId);
}
