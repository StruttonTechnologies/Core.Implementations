using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;

namespace StruttonTechnologies.Core.Repositories.Crud;

/// <summary>
/// Provides CRUD operations for entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Returns an <see cref="IQueryable{T}"/> for the entity set, 
    /// automatically filtering out entities with <c>IsDeleted</c> set to <c>true</c> if such a property exists.
    /// </summary>
    /// <returns>An <see cref="IQueryable{T}"/> for querying entities.</returns>
    public IQueryable<TEntity> Query()
    {
        var query = DbSet.AsQueryable();

        // Add the where clause if the entity has a property called IsDeleted
        var isDeletedProp = typeof(TEntity).GetProperty("IsDeleted");
        if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool))
        {
            query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
        }

        return query;
    }

    /// <summary>
    /// Returns an <see cref="IQueryable{T}"/> for the entity set with no tracking,
    /// automatically filtering out entities with <c>IsDeleted</c> set to <c>true</c> if such a property exists.
    /// </summary>
    /// <returns>An <see cref="IQueryable{T}"/> for querying entities without tracking.</returns>
    public IQueryable<TEntity> QueryNoTracking()
    {
        var query = DbSet.AsNoTracking();
        // Add the where clause if the entity has a property called IsDeleted
        var isDeletedProp = typeof(TEntity).GetProperty("IsDeleted");
        if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool))
        {
            query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
        }

        return query;
    }

    /// <summary>
    /// Checks if any entity exists that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if an entity exists; otherwise, false.</returns>
    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryWithExceptionHandlingAsync(async () =>
        {
            return await DbSet.AnyAsync(filter, cancellationToken);
        });
    }

    /// <summary>
    /// Checks if any entity exists with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to check for existence.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains <c>true</c> if an entity with the specified identifier exists; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryWithExceptionHandlingAsync(async () =>
        {
            return await DbSet.AnyAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), cancellationToken);
        });
    }

    /// <summary>
    /// Adapter implementation required by IQueryHelpers<TEntity, Guid>.
    /// Delegates to the TKey-based overload when the repository key type is Guid.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (typeof(TKey) == typeof(Guid))
        {
            // safe cast because we confirmed TKey is Guid
            return await ExistsAsync((TKey)(object)id, cancellationToken);
        }

        throw new NotSupportedException("ExistsAsync(Guid) is only supported when repository TKey is Guid.");
    }
}

