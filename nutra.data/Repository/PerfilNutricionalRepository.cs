using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.Usuario;

namespace Nutra.Repository;

public class PerfilNutricionalRepository : BaseRepository<PerfilNutricional>, IPerfilNutricionalRepository
{
    public PerfilNutricionalRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<PerfilNutricional?> ObterPorUsuarioIdAsync(string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<PerfilNutricional?> ObterComColecoesAsync(string userId)
    {
        return await _dbSet
            .Include(p => p.RestricoesAlimentares)
            .Include(p => p.EquipamentoDisponivel)
            .Include(p => p.PreferenciasAlimentares)
            .Include(p => p.HistoricosClinico)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<bool> ExistePorEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(p => p.User.Email == email);
    }
}
