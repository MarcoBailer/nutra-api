using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class RegistroAlimentarRepository : BaseRepository<RegistroAlimentar>, IRegistroAlimentarRepository
{
    public RegistroAlimentarRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<RegistroAlimentar?> ObterPorIdEUsuarioAsync(long registroId, string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.Id == registroId && r.UserId == userId);
    }

    public async Task<IEnumerable<RegistroAlimentar>> ListarPorUsuarioEPeriodoAsync(
        string userId, DateTime inicio, DateTime fim)
    {
        return await NoPeriodo(userId, inicio, fim).ToListAsync();
    }

    public async Task<IEnumerable<RegistroAlimentar>> ListarComItemPlanoPorUsuarioEPeriodoAsync(
        string userId, DateTime inicio, DateTime fim)
    {
        return await NoPeriodo(userId, inicio, fim)
            .Include(r => r.ItemRefeicaoPlano)
            .ToListAsync();
    }

    private IQueryable<RegistroAlimentar> NoPeriodo(string userId, DateTime inicio, DateTime fim)
    {
        return _dbSet
            .Where(r => r.UserId == userId && r.DataConsumo >= inicio && r.DataConsumo < fim)
            .OrderBy(r => r.DataConsumo);
    }
}
