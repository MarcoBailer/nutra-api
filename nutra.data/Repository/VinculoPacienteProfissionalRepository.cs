using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models.Usuario;

namespace Nutra.Repository;

public class VinculoPacienteProfissionalRepository
    : BaseRepository<VinculoPacienteProfissional>, IVinculoPacienteProfissionalRepository
{
    public VinculoPacienteProfissionalRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<bool> ExisteVinculoAtivoAsync(string profissionalUserId, string pacienteUserId)
    {
        return await _dbSet.AnyAsync(v =>
            v.Profissional.UserId == profissionalUserId &&
            v.PacienteUserId == pacienteUserId &&
            v.Status == EStatusVinculo.Ativo);
    }

    public async Task<bool> ExisteVinculoEmAbertoAsync(string profissionalUserId, string pacienteUserId)
    {
        return await _dbSet.AnyAsync(v =>
            v.Profissional.UserId == profissionalUserId &&
            v.PacienteUserId == pacienteUserId &&
            (v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente));
    }

    public async Task<bool> ExisteVinculoEmAbertoPorPerfilAsync(int perfilProfissionalId, string pacienteUserId)
    {
        return await _dbSet.AnyAsync(v =>
            v.PerfilProfissionalId == perfilProfissionalId &&
            v.PacienteUserId == pacienteUserId &&
            (v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente));
    }

    public async Task<int> ContarAtivosPorPerfilAsync(int perfilProfissionalId)
    {
        return await _dbSet.CountAsync(v =>
            v.PerfilProfissionalId == perfilProfissionalId &&
            v.Status == EStatusVinculo.Ativo);
    }

    public async Task<int> ContarEmAbertoPorPerfilAsync(int perfilProfissionalId)
    {
        return await _dbSet.CountAsync(v =>
            v.PerfilProfissionalId == perfilProfissionalId &&
            (v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente));
    }

    public async Task<VinculoPacienteProfissional?> ObterConvitePendenteAsync(int vinculoId, string pacienteUserId)
    {
        return await _dbSet.FirstOrDefaultAsync(v =>
            v.Id == vinculoId &&
            v.PacienteUserId == pacienteUserId &&
            v.Status == EStatusVinculo.Pendente);
    }

    public async Task<VinculoPacienteProfissional?> ObterVinculoAtivoDoParticipanteAsync(int vinculoId, string userId)
    {
        return await _dbSet
            .Include(v => v.Profissional)
            .FirstOrDefaultAsync(v =>
                v.Id == vinculoId &&
                (v.PacienteUserId == userId || v.Profissional.UserId == userId) &&
                v.Status == EStatusVinculo.Ativo);
    }

    public async Task<IEnumerable<VinculoPacienteProfissional>> ListarPacientesDoProfissionalAsync(
        string profissionalUserId)
    {
        return await _dbSet
            .Include(v => v.Paciente)
                .ThenInclude(p => p.PerfilAtivo)
            .Include(v => v.Clinica)
            .Where(v => v.Profissional.UserId == profissionalUserId &&
                        (v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente))
            .ToListAsync();
    }

    public async Task<IEnumerable<VinculoPacienteProfissional>> ListarProfissionaisDoPacienteAsync(
        string pacienteUserId)
    {
        return await _dbSet
            .Include(v => v.Profissional)
                .ThenInclude(p => p.User)
            .Include(v => v.Clinica)
            .Where(v => v.PacienteUserId == pacienteUserId &&
                        (v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente))
            .ToListAsync();
    }
}
