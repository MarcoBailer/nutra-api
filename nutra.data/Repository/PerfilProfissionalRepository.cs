using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.Usuario;

namespace Nutra.Repository;

public class PerfilProfissionalRepository : BaseRepository<PerfilProfissional>, IPerfilProfissionalRepository
{
    public PerfilProfissionalRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<PerfilProfissional?> ObterPorUsuarioIdAsync(string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<PerfilProfissional?> ObterComUsuarioEAssinaturaAsync(string userId)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.AssinaturaAtiva)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<bool> ExistePorUsuarioIdAsync(string userId)
    {
        return await _dbSet.AnyAsync(p => p.UserId == userId);
    }

    public async Task<bool> ExistePorCrnAsync(string crn)
    {
        return await _dbSet.AnyAsync(p => p.CRN == crn);
    }
}
