using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class PlanoAlimentarRepository : BaseRepository<PlanoAlimentar>, IPlanoAlimentarRepository
{
    public PlanoAlimentarRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<PlanoAlimentar?> ObterCompletoPorIdAsync(int planoId)
    {
        return await Completo().FirstOrDefaultAsync(p => p.Id == planoId);
    }

    public async Task<PlanoAlimentar?> ObterCompletoPorIdEUsuarioAsync(int planoId, string userId)
    {
        return await Completo()
            .FirstOrDefaultAsync(p => p.Id == planoId && p.PerfilNutricional.UserId == userId);
    }

    public async Task<PlanoAlimentar?> ObterCompletoPorIdEPerfilAsync(int planoId, int perfilNutricionalId)
    {
        return await Completo()
            .FirstOrDefaultAsync(p => p.Id == planoId && p.PerfilNutricionalId == perfilNutricionalId);
    }

    public async Task<PlanoAlimentar?> ObterCompletoAtivoPorUsuarioAsync(string userId)
    {
        return await Completo()
            .FirstOrDefaultAsync(p => p.PerfilNutricional.UserId == userId && p.Status == EStatusPlano.Ativo);
    }

    public async Task<PlanoAlimentar?> ObterPorIdEPerfilAsync(int planoId, int perfilNutricionalId)
    {
        return await _dbSet.FirstOrDefaultAsync(p =>
            p.Id == planoId && p.PerfilNutricionalId == perfilNutricionalId);
    }

    public async Task<PlanoAlimentar?> ObterAtivoPorPerfilAsync(int perfilNutricionalId)
    {
        return await _dbSet.FirstOrDefaultAsync(p =>
            p.PerfilNutricionalId == perfilNutricionalId && p.Status == EStatusPlano.Ativo);
    }

    public async Task<PlanoAlimentar?> ObterAtivoComRefeicoesEItensAsync(int perfilNutricionalId)
    {
        return await _dbSet
            .Include(p => p.RefeicoesPlanejadas)
                .ThenInclude(r => r.Itens)
            .FirstOrDefaultAsync(p =>
                p.PerfilNutricionalId == perfilNutricionalId && p.Status == EStatusPlano.Ativo);
    }

    public async Task<IEnumerable<PlanoAlimentar>> ListarPorPerfilAsync(int perfilNutricionalId)
    {
        return await _dbSet
            .Include(p => p.ProfissionalResponsavel)
            .Include(p => p.RefeicoesPlanejadas)
                .ThenInclude(r => r.Itens)
            .Where(p => p.PerfilNutricionalId == perfilNutricionalId)
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlanoAlimentar>> ListarAtivosPorPerfilAsync(int perfilNutricionalId)
    {
        return await _dbSet
            .Where(p => p.PerfilNutricionalId == perfilNutricionalId && p.Status == EStatusPlano.Ativo)
            .ToListAsync();
    }

    /// <summary>
    /// Árvore completa do plano. Existe porque cinco consultas repetiam
    /// exatamente esta cadeia de <c>Include</c> — só o filtro muda.
    /// </summary>
    private IQueryable<PlanoAlimentar> Completo()
    {
        return _dbSet
            .Include(p => p.PerfilNutricional)
            .Include(p => p.ProfissionalResponsavel)
            .Include(p => p.RefeicoesPlanejadas.OrderBy(r => r.Ordem))
                .ThenInclude(r => r.Itens.OrderBy(i => i.Ordem))
                    .ThenInclude(i => i.SubstituicoesEquivalentes);
    }
}
