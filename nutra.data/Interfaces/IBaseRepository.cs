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

    /// <summary>
    /// Uma página de resultados mais o total de registros que casam com o filtro.
    /// <para>
    /// <paramref name="predicates"/> é uma coleção porque os filtros entram em
    /// conjunção (AND): é assim que a busca por várias palavras é expressa sem
    /// montar árvore de expressão na mão.
    /// </para>
    /// <para>
    /// <paramref name="orderBy"/> é obrigatório: paginar sem <c>ORDER BY</c> deixa
    /// a ordem a critério do banco, e a mesma linha pode aparecer em duas páginas
    /// ou em nenhuma.
    /// </para>
    /// </summary>
    Task<(IReadOnlyList<T> Items, int TotalCount)> FindPagedAsync<TKey>(
        IReadOnlyCollection<Expression<Func<T, bool>>> predicates,
        Expression<Func<T, TKey>> orderBy,
        int pageNumber,
        int pageSize);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    void Add(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
