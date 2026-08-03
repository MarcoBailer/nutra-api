using Nutra.Models.Usuario;

namespace Nutra.Interfaces;

public interface IMetaNutricionalRepository : IBaseRepository<MetaNutricional>
{
    Task<MetaNutricional?> ObterPorPerfilIdAsync(int perfilNutricionalId);
}
