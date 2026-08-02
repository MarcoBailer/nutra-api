using Nutra.Enum;
using Nutra.Models;
using Nutra.Models.Dtos;

namespace Nutra.Interfaces;

/// <summary>
/// Serviço de gestão de profissionais nutricionistas.
/// </summary>
public interface INutricionista
{
    // --- Perfil profissional ---
    Task<RetornoPadrao> CadastrarNutricionistaAsync(CadastroNutricionistaDto dto);
    Task<RetornoPadrao<PerfilProfissionalDto>> ObterPerfilProfissionalAsync(string userId);
    Task<RetornoPadrao> AtualizarPerfilProfissionalAsync(string userId, UpdatePerfilProfissionalDto dto);

    // --- Clínicas ---
    Task<RetornoPadrao> CriarClinicaAsync(string userId, ClinicaDto dto);
    Task<RetornoPadrao> AtualizarClinicaAsync(string userId, int clinicaId, ClinicaDto dto);
    Task<RetornoPadrao> RemoverClinicaAsync(string userId, int clinicaId);
    Task<RetornoPadrao<List<ClinicaDto>>> ListarClinicasAsync(string userId);

    // --- Gestão de pacientes (vínculos) ---
    Task<RetornoPadrao> EnviarConvitePacienteAsync(string nutricionistaUserId, ConviteVinculoDto dto);
    Task<RetornoPadrao> ResponderConviteAsync(string pacienteUserId, int vinculoId, bool aceitar);
    Task<RetornoPadrao> EncerrarVinculoAsync(string userId, int vinculoId);
    Task<RetornoPadrao<List<PacienteResumoDto>>> ListarPacientesAsync(string nutricionistaUserId);
    Task<RetornoPadrao<List<PacienteResumoDto>>> ListarNutricionistasAsync(string pacienteUserId);

    // --- Assinatura ---
    Task<RetornoPadrao> AtualizarPlanoAsync(string userId, EPlanoAssinatura novoPlano);
}
