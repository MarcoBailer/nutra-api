using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;
using Nutra.Models.Usuario;

namespace Nutra.Repository;

public class RegistroBiometricoRepository : BaseRepository<RegistroBiometrico>, IRegistroBiometricoRepository
{
    public RegistroBiometricoRepository(AlimentosContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RegistroBiometrico>> ListarPorPerfilAsync(int perfilNutricionalId)
    {
        return await _dbSet
            .Where(r => r.PerfilNutricionalId == perfilNutricionalId)
            .OrderByDescending(r => r.Data)
            .ToListAsync();
    }
}
