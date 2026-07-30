using Nutra.Enum;
using Nutra.Models;
using Nutra.Models.Dtos;

namespace Nutra.Interfaces
{
    public interface IUserProfile
    {
        // --- Perfil Nutricional ---
        Task<PerfilNutricionalDto> GetPerfilNutricional(string userId);
        Task<RetornoPadrao> PostPerfilNutricional(PerfilNutricionalDto perfilNutricional);
        Task<RetornoPadrao> AtualizarPerfilNutricional(string userId, PerfilNutricionalDto dto);

        // --- Preferências ---
        Task<RetornoPadrao> PostPreferenciaAlimentar(string userId, int id, ETipoTabela tabela, ETipoPreferencia afinidade);
        Task<RetornoPadrao> RemoverPreferenciaAlimentar(string userId, int preferenciaId);

        // --- Biométrico ---
        Task<RetornoPadrao> PostRegistroBiometrico(string userId, RegistroBiometricoDto registroBiometricoDto);
        Task<List<RegistroBiometricoDto>> ListarHistoricoBiometrico(string userId);

        // --- Histórico Clínico ---
        Task<RetornoPadrao> AdicionarHistoricoClinico(string userId, HistoricoClinicoDto dto);
        Task<RetornoPadrao> AtualizarHistoricoClinico(string userId, int id, HistoricoClinicoDto dto);
        Task<RetornoPadrao> RemoverHistoricoClinico(string userId, int id);
        Task<List<HistoricoClinicoDto>> ListarHistoricoClinico(string userId);

        // --- Anamnese Alimentar ---
        Task<RetornoPadrao> SalvarAnamneseAlimentar(string userId, AnamneseAlimentarDto dto);
        Task<AnamneseAlimentarDto?> ObterUltimaAnamnese(string userId);
        Task<List<AnamneseAlimentarDto>> ListarAnamneses(string userId);
    }
}
