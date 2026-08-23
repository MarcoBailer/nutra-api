using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Interfaces;

namespace Nutra.Repository;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AlimentosContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AlimentosContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, int limite)
    {
        return await _dbSet.Where(predicate).Take(limite).ToListAsync();
    }

    public async Task<(IReadOnlyList<T> Items, int TotalCount)> FindPagedAsync<TKey>(
        IReadOnlyCollection<Expression<Func<T, bool>>> predicates,
        Expression<Func<T, TKey>> orderBy,
        int pageNumber,
        int pageSize)
    {
        IQueryable<T> query = _dbSet;

        foreach (var predicate in predicates)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(orderBy)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}
