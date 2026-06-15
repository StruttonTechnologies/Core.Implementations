using Microsoft.EntityFrameworkCore;
using StruttonTechnologies.Core.Domain.Entities.Base;
using StruttonTechnologies.Core.Repositories.Base;
using StruttonTechnologies.Core.Repositories.Contracts.Base;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;
using System.Linq.Expressions;

namespace StruttonTechnologies.Core.Repositories.Crud;

public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Retrieves an entity by its ID with optional navigation property includes.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <param name="includeProperties">Navigation properties to include in the query results.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the entity found,
    /// or <c>null</c> if no entity with the specified ID exists.
    /// </returns>
    public async Task<TEntity?> GetByIdAsync(
        TKey id,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return await ExecuteQueryWithExceptionHandlingAsync(async () =>
        {
            IQueryable<TEntity> query = DbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id").Equals(id), cancellationToken);
        });
    }
}

