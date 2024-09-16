using System.Linq.Expressions;

namespace CashFlow.Entries.Domain.Gateways
{
    public interface IGenericRepository<T> where T : class
    {
        #region Write operations
        Task AddAsync(T entity, CancellationToken cancellationToken);
        Task AddManyAsync(ICollection<T> entities, CancellationToken cancellationToken);

        Task<bool> UpdateAsync<TValue>(Expression<Func<T, bool>> query, TValue value, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Expression<Func<T, bool>> query, CancellationToken cancellationToken = default);

        #endregion Write operations

        #region Read operations
        Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken);
        Task<ICollection<T>> WhereAsync(Expression<Func<T, bool>> query = null, CancellationToken cancellationToken = default);
        Task<ICollection<TOutput>> WhereAsync<TOutput>(Expression<Func<T, bool>> query, CancellationToken cancellationToken = default);
        Task<ICollection<TOutput>> WhereAsync<TOutput>(Expression<Func<T, bool>> query, Expression<Func<T, TOutput>> selector, CancellationToken cancellationToken = default);
        Task<T> FirstAsync(Expression<Func<T, bool>> query, CancellationToken cancellationToken);
        Task<TOutput> FirstAsync<TOutput>(Expression<Func<T, bool>> expression, Expression<Func<T, TOutput>> selector, CancellationToken cancellationToken = default);

        Task<List<TOutput>> GroupByAsync<TOutput, TGroup>(Expression<Func<T, bool>> query, Expression<Func<T, TGroup>> keySelector, Expression<Func<IGrouping<TGroup, T>, TOutput>> selector, CancellationToken cancellationToken = default);

        Task<long> CountAsync(Expression<Func<T, bool>> query, CancellationToken cancellationToken = default);
        Task<long> SumAsync(Expression<Func<T, bool>> query, Expression<Func<T, long>> selector, CancellationToken cancellationToken = default);

        #endregion Read operations
    }
}
