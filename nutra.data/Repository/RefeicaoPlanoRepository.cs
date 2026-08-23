using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class RefeicaoPlanoRepository : BaseRepository<RefeicaoPlano>, IRefeicaoPlanoRepository
{
    public RefeicaoPlanoRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<RefeicaoPlano?> ObterComItensESubstituicoesAsync(int refeicaoId, int perfilNutricionalId)
    {
        return await _dbSet
            .Include(r => r.PlanoAlimentar)
            .Include(r => r.Itens)
                .ThenInclude(i => i.SubstituicoesEquivalentes)
            .FirstOrDefaultAsync(r =>
                r.Id == refeicaoId && r.PlanoAlimentar.PerfilNutricionalId == perfilNutricionalId);
    }

    public async Task<RefeicaoPlano?> ObterComItensAsync(int refeicaoId, int perfilNutricionalId)
    {
        return await _dbSet
            .Include(r => r.PlanoAlimentar)
            .Include(r => r.Itens)
            .FirstOrDefaultAsync(r =>
                r.Id == refeicaoId && r.PlanoAlimentar.PerfilNutricionalId == perfilNutricionalId);
    }

    public async Task<RefeicaoPlano?> ObterComItensPorIdAsync(int refeicaoId)
    {
        return await _dbSet
            .Include(r => r.Itens)
            .FirstOrDefaultAsync(r => r.Id == refeicaoId);
    }
}
