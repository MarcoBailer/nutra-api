using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.Usuario;

namespace Nutra.Repository;

public class MetaNutricionalRepository : BaseRepository<MetaNutricional>, IMetaNutricionalRepository
{
    public MetaNutricionalRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<MetaNutricional?> ObterPorPerfilIdAsync(int perfilNutricionalId)
    {
        return await _dbSet.FirstOrDefaultAsync(m => m.PerfilNutricionalId == perfilNutricionalId);
    }
}
