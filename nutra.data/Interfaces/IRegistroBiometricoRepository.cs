using Nutra.Models.Usuario;

namespace Nutra.Interfaces;

public interface IRegistroBiometricoRepository : IBaseRepository<RegistroBiometrico>
{
    /// <summary>Mais recentes primeiro.</summary>
    Task<IEnumerable<RegistroBiometrico>> ListarPorPerfilAsync(int perfilNutricionalId);
}
