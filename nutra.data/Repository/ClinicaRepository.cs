using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.Usuario;

namespace Nutra.Repository;

public class ClinicaRepository : BaseRepository<Clinica>, IClinicaRepository
{
    public ClinicaRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<Clinica?> ObterPorIdEUsuarioAsync(int clinicaId, string userId)
    {
        return await _dbSet
            .Include(c => c.PerfilProfissional)
            .FirstOrDefaultAsync(c => c.Id == clinicaId && c.PerfilProfissional.UserId == userId);
    }

    public async Task<IEnumerable<Clinica>> ListarAtivasPorUsuarioAsync(string userId)
    {
        return await _dbSet
            .Where(c => c.PerfilProfissional.UserId == userId && c.Ativo)
            .ToListAsync();
    }

    public async Task<int> ContarAtivasPorPerfilAsync(int perfilProfissionalId)
    {
        return await _dbSet.CountAsync(c => c.PerfilProfissionalId == perfilProfissionalId && c.Ativo);
    }

    public async Task<bool> ExisteAtivaAsync(int clinicaId, int perfilProfissionalId)
    {
        return await _dbSet.AnyAsync(c =>
            c.Id == clinicaId && c.PerfilProfissionalId == perfilProfissionalId && c.Ativo);
    }
}
