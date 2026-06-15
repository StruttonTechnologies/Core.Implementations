using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace StruttonTechnologies.Core.Repositories.Crud;

public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Asynchronously retrieves a collection of entities from the database with optional filtering, sorting, pagination, batching, and eager loading of related entities.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used for sorting.</typeparam>
    /// <param name="isSorted">Indicates whether the result should be sorted.</param>
    /// <param name="orderBy">An expression to order the results by.</param>
    /// <param name="ascending">Indicates whether the sorting should be ascending (true) or descending (false).</param>
    /// <param name="isPaginated">Indicates whether the result should be paginated.</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="isBatched">Indicates whether batching should be used (not implemented in this method).</param>
    /// <param name="batchSize">The size of each batch (not implemented in this method).</param>
    /// <param name="filter">An optional filter expression to apply to the query.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <param name="includeProperties">Expressions specifying related entities to include in the query results.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> of entities.</returns>

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default
)
    {
        return await GetAllAsync<TKey>(
            isSorted: true,
            orderBy: e => EF.Property<TKey>(e, "Id")
        );
    }


    public async Task<IEnumerable<TEntity>> GetAllAsync<TSortKey>(
        bool isSorted = false,
        Expression<Func<TEntity, TSortKey>>? orderBy = null,
        bool ascending = true,
        bool isPaginated = false,
        int pageNumber = 1,
        int pageSize = 100,
        bool isBatched = false,
        int batchSize = 100,
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return await ExecuteQueryWithExceptionHandlingAsync(async () =>
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (isSorted && orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            if (isPaginated)
            {
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }

            return await query.ToListAsync(cancellationToken);
        });
    }
}

