using Nutra.Enum;
using Nutra.Models;
using Nutra.Models.Dtos;

namespace Nutra.Interfaces
{
    public interface IUserProfile
    {
        Task<PerfilNutricionalDto> GetPerfilNutricional(string userId);
        Task<RetornoPadrao> PostPerfilNutricional(PerfilNutricionalDto perfilNutricional);
        Task<RetornoPadrao> PostPreferenciaAlimentar(string userId, int id, ETipoTabela tabela, ETipoPreferencia afinidade);
        Task<RetornoPadrao> PostRegistroBiometrico(string userId, RegistroBiometricoDto registroBiometricoDto);
    }
}
