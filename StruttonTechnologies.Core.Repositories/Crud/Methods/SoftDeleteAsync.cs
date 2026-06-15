using StruttonTechnologies.Core.Domain.Entities.Base;
using StruttonTechnologies.Core.Repositories.Base;
using StruttonTechnologies.Core.Repositories.Contracts.Base;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Repositories.Crud;

/// <summary>
/// Provides CRUD operations for entities, including soft deletion.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Soft deletes an entity by setting its <c>IsDeleted</c> property to <c>true</c>.
    /// </summary>
    /// <param name="id">The identifier of the entity to soft delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// The soft-deleted entity if found; otherwise, <c>null</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the entity type does not have a writable <c>IsDeleted</c> boolean property.
    /// </exception>
    public async Task<TEntity?> SoftDeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
            return null;

        var prop = typeof(TEntity).GetProperty("IsDeleted");
        if (prop is null || !prop.CanWrite || prop.PropertyType != typeof(bool))
            throw new InvalidOperationException($"Entity type '{typeof(TEntity).Name}' does not support soft deletion.");

        prop.SetValue(entity, true);

        return await ExecuteWithExceptionHandlingAsync(async () =>
        {
            DbSet.Update(entity);
            await Context.SaveChangesAsync(cancellationToken);
            return entity;
        });
    }
}
