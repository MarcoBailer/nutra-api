using Nutra.Models.Usuario;

namespace Nutra.Interfaces;

public interface IApplicationUserService
{
    Task<ApplicationUser?> FindByIdAsync(string userId);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser> CreateAsync(ApplicationUser user);
    Task<bool> UpdateAsync(ApplicationUser user);
}
