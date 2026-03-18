using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.Usuario;

namespace Nutra.Services;

public class ApplicationUserService : IApplicationUserService
{
    private readonly AlimentosContext _context;

    public ApplicationUserService(AlimentosContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> FindByIdAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return null;
        }

        return await _context.ApplicationUsers
            .FirstOrDefaultAsync(user => user.Id == parsedUserId);
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return await _context.ApplicationUsers
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<bool> UpdateAsync(ApplicationUser user)
    {
        var existing = await _context.ApplicationUsers
            .FirstOrDefaultAsync(current => current.Id == user.Id);

        if (existing == null)
        {
            return false;
        }

        _context.Entry(existing).CurrentValues.SetValues(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public Task CreateAsync(ApplicationUser user)
    {
        throw new NotImplementedException("CreateAsync ainda não foi implementado. Defina se a criação será local ou delegada ao serviço de autenticação.");
    }
}
