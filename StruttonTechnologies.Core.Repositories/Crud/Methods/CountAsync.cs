using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace StruttonTechnologies.Core.Repositories.Crud;

public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Counts the number of entities that match the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities.</returns>
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryWithExceptionHandlingAsync(async () =>
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.CountAsync(cancellationToken);
        });
    }
}

