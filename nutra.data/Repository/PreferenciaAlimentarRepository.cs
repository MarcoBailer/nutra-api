using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class PreferenciaAlimentarRepository : BaseRepository<PreferenciaAlimentar>, IPreferenciaAlimentarRepository
{
    public PreferenciaAlimentarRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<PreferenciaAlimentar?> ObterPorPerfilEAlimentoAsync(
        int perfilNutricionalId, int alimentoId, ETipoTabela tabela)
    {
        return await _dbSet.FirstOrDefaultAsync(p =>
            p.PerfilNutricionalId == perfilNutricionalId &&
            p.AlimentoId == alimentoId &&
            p.Tabela == tabela);
    }

    public async Task<PreferenciaAlimentar?> ObterPorIdEUsuarioAsync(int preferenciaId, string userId)
    {
        return await _dbSet
            .Include(p => p.Perfil)
            .FirstOrDefaultAsync(p => p.Id == preferenciaId && p.Perfil.UserId == userId);
    }
}
