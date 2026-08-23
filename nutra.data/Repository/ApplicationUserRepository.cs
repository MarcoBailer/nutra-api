using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.Usuario;

namespace Nutra.Repository;

public class ApplicationUserRepository : IApplicationUserRepository
{
    private readonly AlimentosContext _context;

    public ApplicationUserRepository(AlimentosContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> FindByIdAsync(string userId)
    {
        return await _context.ApplicationUsers.FindAsync(userId);
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return await _context.ApplicationUsers.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<ApplicationUser> CreateAsync(ApplicationUser user)
    {
        _context.ApplicationUsers.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(ApplicationUser user)
    {
        var existing = await _context.ApplicationUsers.FirstOrDefaultAsync(current => current.Id == user.Id);

        if (existing == null)
        {
            return false;
        }

        _context.Entry(existing).CurrentValues.SetValues(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
