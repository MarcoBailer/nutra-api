namespace Nutra.Interfaces;

/// <summary>
/// Ponto único de commit. O handle da transação fica aqui dentro — devolvê-lo
/// obrigaria o serviço a conhecer <c>IDbContextTransaction</c>, que é tipo do EF.
/// </summary>
public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<bool> SaveChangesAsync();
}
