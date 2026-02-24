using Nutra.Models;
using Nutra.Models.Dtos;
using Nutra.Models.Dtos.Registro;

namespace Nutra.Interfaces
{
    public interface IAccounts
    {
        Task<RetornoPadrao> AtualizarPerfilAsync(string userId, UpdateProfileDto dto);
        Task<RetornoPadrao> DesativarContaAsync(string userId);
        Task<RetornoPadrao> ReativarContaAsync(string userId);
    }
}
