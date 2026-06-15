using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace StruttonTechnologies.Core.Repositories.Crud;

public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Retrieves entities from the database based on the specified filter, sorting, pagination, and included properties.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used for sorting.</typeparam>
    /// <param name="filter">An optional filter expression to apply to the query.</param>
    /// <param name="isSorted">Indicates whether sorting should be applied.</param>
    /// <param name="orderBy">An optional expression specifying the property to sort by.</param>
    /// <param name="ascending">Indicates whether sorting should be in ascending order. Default is true.</param>
    /// <param name="isPaginated">Indicates whether pagination should be applied.</param>
    /// <param name="pageNumber">The page number to retrieve. Default is 1.</param>
    /// <param name="pageSize">The number of items per page. Default is 100.</param>
    /// <param name="isBatched">Indicates whether batching should be applied. (Currently unused)</param>
    /// <param name="batchSize">The size of each batch. Default is 100. (Currently unused)</param>
    /// <param name="cancellationToken">A cancellation token for the async operation.</param>
    /// <param name="includeProperties">Navigation properties to include in the query.</param>
    /// <returns>A collection of entities matching the specified criteria.</returns>
    public async Task<IEnumerable<TEntity>> GetByFilterAsync<TSortKey>(
        Expression<Func<TEntity, bool>>? filter = null,
        bool isSorted = false,
        Expression<Func<TEntity, TSortKey>>? orderBy = null,
        bool ascending = true,
        bool isPaginated = false,
        int pageNumber = 1,
        int pageSize = 100,
        bool isBatched = false,
        int batchSize = 100,
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
                query = ascending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }

            if (isPaginated)
            {
                query = query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
            }

            return await query.ToListAsync(cancellationToken);
        });
    }
}

