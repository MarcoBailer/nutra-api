using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class AnamneseAlimentarRepository : BaseRepository<AnamneseAlimentar>, IAnamneseAlimentarRepository
{
    public AnamneseAlimentarRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<AnamneseAlimentar?> ObterUltimaPorPerfilAsync(int perfilNutricionalId)
    {
        return await _dbSet
            .Where(a => a.PerfilNutricionalId == perfilNutricionalId)
            .OrderByDescending(a => a.DataPreenchimento)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AnamneseAlimentar>> ListarPorPerfilAsync(int perfilNutricionalId)
    {
        return await _dbSet
            .Where(a => a.PerfilNutricionalId == perfilNutricionalId)
            .OrderByDescending(a => a.DataPreenchimento)
            .ToListAsync();
    }
}
