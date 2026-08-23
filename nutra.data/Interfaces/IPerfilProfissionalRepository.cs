using Nutra.Models.Usuario;

namespace Nutra.Interfaces;

public interface IPerfilProfissionalRepository : IBaseRepository<PerfilProfissional>
{
    Task<PerfilProfissional?> ObterPorUsuarioIdAsync(string userId);

    Task<PerfilProfissional?> ObterComUsuarioEAssinaturaAsync(string userId);

    Task<bool> ExistePorUsuarioIdAsync(string userId);

    Task<bool> ExistePorCrnAsync(string crn);
}
