using StruttonTechnologies.Core.ToolKit.Exceptions;

namespace StruttonTechnologies.Core.Repositories.Crud;

public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Asynchronously updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the updated entity.
    /// </returns>
    /// <exception cref="ValidationException">Thrown when the entity is null.</exception>
    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
        {
            throw new ValidationException("Entity cannot be null.");
        }

        return await ExecuteWithExceptionHandlingAsync(async () =>
        {
            DbSet.Update(entity);
            await Context.SaveChangesAsync(cancellationToken);
            return entity;
        });
    }
}
