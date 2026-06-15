using StruttonTechnologies.Core.ToolKit.Exceptions;

namespace StruttonTechnologies.Core.Repositories.Crud;


public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity == null)
        {
            throw new ValidationException("Entity cannot be null.");
        }

        return await ExecuteWithExceptionHandlingAsync(async () =>
        {
            await DbSet.AddAsync(entity, cancellationToken);
            return entity;
        });
    }
}

