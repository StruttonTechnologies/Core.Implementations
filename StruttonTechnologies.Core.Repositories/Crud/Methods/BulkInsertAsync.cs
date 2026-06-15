namespace StruttonTechnologies.Core.Repositories.Crud;

public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Inserts multiple entities in bulk and returns the inserted entities.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation, containing the inserted entities.</returns>
    public async Task<IEnumerable<TEntity>> BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities), "Entities cannot be null.");
        }

        List<TEntity> entityList = entities is List<TEntity> list ? list : new List<TEntity>(entities);

        return await ExecuteWithExceptionHandlingAsync(async () =>
        {
            await DbSet.AddRangeAsync(entityList, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            return entityList;
        });
    }
}

