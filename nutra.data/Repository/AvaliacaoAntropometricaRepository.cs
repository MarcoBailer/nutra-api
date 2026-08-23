using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.RegraNutricional;

namespace Nutra.Repository;

public class AvaliacaoAntropometricaRepository
    : BaseRepository<AvaliacaoAntropometrica>, IAvaliacaoAntropometricaRepository
{
    public AvaliacaoAntropometricaRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<AvaliacaoAntropometrica?> ObterPorIdEPerfilAsync(int id, int perfilNutricionalId)
    {
        return await _dbSet.FirstOrDefaultAsync(a =>
            a.Id == id && a.PerfilNutricionalId == perfilNutricionalId);
    }

    public async Task<AvaliacaoAntropometrica?> ObterCompletaPorIdEPerfilAsync(int id, int perfilNutricionalId)
    {
        return await _dbSet
            .Include(a => a.ProfissionalResponsavel)
            .Include(a => a.FotosProgresso)
            .FirstOrDefaultAsync(a => a.Id == id && a.PerfilNutricionalId == perfilNutricionalId);
    }

    public async Task<AvaliacaoAntropometrica?> ObterComFotosPorIdEPerfilAsync(int id, int perfilNutricionalId)
    {
        return await _dbSet
            .Include(a => a.FotosProgresso)
            .FirstOrDefaultAsync(a => a.Id == id && a.PerfilNutricionalId == perfilNutricionalId);
    }

    public async Task<IEnumerable<AvaliacaoAntropometrica>> ListarPorPerfilAsync(int perfilNutricionalId)
    {
        return await _dbSet
            .Include(a => a.FotosProgresso)
            .Where(a => a.PerfilNutricionalId == perfilNutricionalId)
            .OrderByDescending(a => a.DataAvaliacao)
            .ToListAsync();
    }
}
