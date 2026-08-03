using Microsoft.EntityFrameworkCore.Storage;
using Nutra.Data;
using Nutra.Interfaces;

namespace Nutra.Repository;

/// <summary>
/// Não implementa <c>IDisposable</c>/<c>IAsyncDisposable</c> de propósito: o
/// <see cref="AlimentosContext"/> é scoped e, ao ser descartado, desfaz
/// transação não confirmada. Uma rede de segurança aqui não cobriria nada a mais.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AlimentosContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AlimentosContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction ??= await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction == null) return;

        await _transaction.CommitAsync();
        await DescartarTransacaoAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null) return;

        await _transaction.RollbackAsync();
        await DescartarTransacaoAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    private async Task DescartarTransacaoAsync()
    {
        if (_transaction == null) return;

        await _transaction.DisposeAsync();
        _transaction = null;
    }
}
