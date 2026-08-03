using System.Linq.Expressions;

namespace Nutra.Interfaces;

/// <summary>
/// Operações genéricas de persistência. Só entra aqui o que é expressável por
/// predicado — nada de <c>IQueryable</c>, <c>Include</c> ou tipo do EF, senão o
/// contexto volta a vazar para a camada de serviço.
/// <para>
/// Consulta que precisa de <c>Include</c>, ordenação ou predicado sobre
/// propriedade de navegação mora no repositório específico da entidade, com
/// nome próprio.
/// </para>
/// <para>
/// Não há <c>SaveChangesAsync</c> aqui: quem confirma é <see cref="IUnitOfWork"/>,
/// ponto único de commit.
/// </para>
/// </summary>
public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>Igual ao <see cref="FindAsync(Expression{Func{T, bool}})"/>, mas o corte acontece no banco.</summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, int limite);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    void Add(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
