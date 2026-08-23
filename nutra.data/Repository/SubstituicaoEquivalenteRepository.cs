using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class SubstituicaoEquivalenteRepository
    : BaseRepository<SubstituicaoEquivalente>, ISubstituicaoEquivalenteRepository
{
    public SubstituicaoEquivalenteRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<SubstituicaoEquivalente?> ObterComItemRefeicaoEPlanoAsync(
        int substituicaoId, int perfilNutricionalId)
    {
        return await _dbSet
            .Include(s => s.ItemRefeicao)
                .ThenInclude(i => i.RefeicaoPlano)
                    .ThenInclude(r => r.PlanoAlimentar)
            .FirstOrDefaultAsync(s =>
                s.Id == substituicaoId &&
                s.ItemRefeicao.RefeicaoPlano.PlanoAlimentar.PerfilNutricionalId == perfilNutricionalId);
    }
}
