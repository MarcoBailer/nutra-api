using Nutra.Models.Usuario;

namespace Nutra.Interfaces;

public interface IClinicaRepository : IBaseRepository<Clinica>
{
    Task<Clinica?> ObterPorIdEUsuarioAsync(int clinicaId, string userId);

    Task<IEnumerable<Clinica>> ListarAtivasPorUsuarioAsync(string userId);

    Task<int> ContarAtivasPorPerfilAsync(int perfilProfissionalId);

    Task<bool> ExisteAtivaAsync(int clinicaId, int perfilProfissionalId);
}
