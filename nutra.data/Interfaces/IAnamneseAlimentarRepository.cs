using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IAnamneseAlimentarRepository : IBaseRepository<AnamneseAlimentar>
{
    Task<AnamneseAlimentar?> ObterUltimaPorPerfilAsync(int perfilNutricionalId);

    /// <summary>Mais recentes primeiro.</summary>
    Task<IEnumerable<AnamneseAlimentar>> ListarPorPerfilAsync(int perfilNutricionalId);
}
