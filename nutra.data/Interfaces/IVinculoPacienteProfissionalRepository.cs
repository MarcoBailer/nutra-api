using Nutra.Models.Usuario;

namespace Nutra.Interfaces;

public interface IVinculoPacienteProfissionalRepository : IBaseRepository<VinculoPacienteProfissional>
{
    Task<bool> ExisteVinculoAtivoAsync(string profissionalUserId, string pacienteUserId);

    /// <summary>Ativo ou pendente — usado onde convite ainda não respondido também conta.</summary>
    Task<bool> ExisteVinculoEmAbertoAsync(string profissionalUserId, string pacienteUserId);

    Task<bool> ExisteVinculoEmAbertoPorPerfilAsync(int perfilProfissionalId, string pacienteUserId);

    Task<int> ContarAtivosPorPerfilAsync(int perfilProfissionalId);

    Task<int> ContarEmAbertoPorPerfilAsync(int perfilProfissionalId);

    Task<VinculoPacienteProfissional?> ObterConvitePendenteAsync(int vinculoId, string pacienteUserId);

    /// <summary>Vínculo ativo em que <paramref name="userId"/> é o paciente ou o profissional.</summary>
    Task<VinculoPacienteProfissional?> ObterVinculoAtivoDoParticipanteAsync(int vinculoId, string userId);

    Task<IEnumerable<VinculoPacienteProfissional>> ListarPacientesDoProfissionalAsync(string profissionalUserId);

    Task<IEnumerable<VinculoPacienteProfissional>> ListarProfissionaisDoPacienteAsync(string pacienteUserId);
}
