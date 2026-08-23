using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class ModeloDietaRepository : BaseRepository<ModeloDieta>, IModeloDietaRepository
{
    public ModeloDietaRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ModeloDieta>> ListarDisponiveisAsync(string? profissionalUserId)
    {
        return await _dbSet
            .Where(m => m.Ativo &&
                (profissionalUserId == null || m.Publico || m.CriadoPorProfissionalId == profissionalUserId))
            .OrderBy(m => m.Nome)
            .ToListAsync();
    }

    public async Task<ModeloDieta?> ObterCompletoAtivoAsync(int modeloId)
    {
        return await _dbSet
            .Include(m => m.Refeicoes)
                .ThenInclude(r => r.Itens)
            .Include(m => m.CriadoPorProfissional)
            .FirstOrDefaultAsync(m => m.Id == modeloId && m.Ativo);
    }

    public async Task<ModeloDieta?> ObterPorIdEProfissionalAsync(int modeloId, string profissionalUserId)
    {
        return await _dbSet.FirstOrDefaultAsync(m =>
            m.Id == modeloId && m.CriadoPorProfissionalId == profissionalUserId);
    }
}
