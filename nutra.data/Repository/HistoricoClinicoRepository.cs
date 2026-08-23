using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class HistoricoClinicoRepository : BaseRepository<HistoricoClinico>, IHistoricoClinicoRepository
{
    public HistoricoClinicoRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<HistoricoClinico?> ObterPorIdEUsuarioAsync(int id, string userId)
    {
        return await _dbSet
            .Include(h => h.PerfilNutricional)
            .FirstOrDefaultAsync(h => h.Id == id && h.PerfilNutricional.UserId == userId);
    }

    public async Task<IEnumerable<HistoricoClinico>> ListarPorPerfilAsync(int perfilNutricionalId)
    {
        return await _dbSet
            .Where(h => h.PerfilNutricionalId == perfilNutricionalId)
            .OrderByDescending(h => h.CriadoEm)
            .ToListAsync();
    }
}
