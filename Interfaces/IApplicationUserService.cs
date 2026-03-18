using Nutra.Models.Usuario;

namespace Nutra.Interfaces;

public interface IApplicationUserService
{
    Task<ApplicationUser?> FindByIdAsync(string userId);
    Task<bool> UpdateAsync(ApplicationUser user);
    Task CreateAsync(ApplicationUser user);
    Task<ApplicationUser?> FindByEmailAsync(string email);
}
