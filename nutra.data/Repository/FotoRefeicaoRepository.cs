using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class FotoRefeicaoRepository : BaseRepository<FotoRefeicao>, IFotoRefeicaoRepository
{
    public FotoRefeicaoRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<FotoRefeicao?> ObterPorIdEUsuarioAsync(int fotoId, string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.Id == fotoId && f.UserId == userId);
    }

    public async Task<IEnumerable<FotoRefeicao>> ListarPorUsuarioEPeriodoAsync(
        string userId, DateTime inicio, DateTime fim)
    {
        return await _dbSet
            .Where(f => f.UserId == userId && f.DataRegistro >= inicio && f.DataRegistro < fim)
            .OrderBy(f => f.DataRegistro)
            .ToListAsync();
    }
}
