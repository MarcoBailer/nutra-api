using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class ItemRefeicaoRepository : BaseRepository<ItemRefeicao>, IItemRefeicaoRepository
{
    public ItemRefeicaoRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<ItemRefeicao?> ObterComRefeicaoEPlanoAsync(int itemId, int perfilNutricionalId)
    {
        return await _dbSet
            .Include(i => i.RefeicaoPlano)
                .ThenInclude(r => r.PlanoAlimentar)
            .Include(i => i.SubstituicoesEquivalentes)
            .FirstOrDefaultAsync(i =>
                i.Id == itemId &&
                i.RefeicaoPlano.PlanoAlimentar.PerfilNutricionalId == perfilNutricionalId);
    }
}
