using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IHistoricoClinicoRepository : IBaseRepository<HistoricoClinico>
{
    Task<HistoricoClinico?> ObterPorIdEUsuarioAsync(int id, string userId);

    /// <summary>Mais recentes primeiro.</summary>
    Task<IEnumerable<HistoricoClinico>> ListarPorPerfilAsync(int perfilNutricionalId);
}
