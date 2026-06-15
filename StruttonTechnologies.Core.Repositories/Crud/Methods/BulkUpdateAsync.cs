namespace StruttonTechnologies.Core.Repositories.Crud;

public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Updates multiple entities in bulk and returns the updated entities.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation, containing the updated entities.</returns>
    public async Task<IEnumerable<TEntity>> BulkUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities), "Entities cannot be null.");
        }

        List<TEntity> entityList = entities is List<TEntity> list ? list : new List<TEntity>(entities);

        return await ExecuteWithExceptionHandlingAsync(async () =>
        {
            DbSet.UpdateRange(entityList);
            await Context.SaveChangesAsync(cancellationToken);
            return entityList;
        });
    }
}

