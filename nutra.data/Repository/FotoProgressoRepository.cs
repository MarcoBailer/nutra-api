using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class FotoProgressoRepository : BaseRepository<FotoProgresso>, IFotoProgressoRepository
{
    public FotoProgressoRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<FotoProgresso?> ObterPorIdEPerfilAsync(int fotoId, int perfilNutricionalId)
    {
        return await _dbSet.FirstOrDefaultAsync(f =>
            f.Id == fotoId &&
            f.AvaliacaoAntropometrica.PerfilNutricionalId == perfilNutricionalId);
    }
}
