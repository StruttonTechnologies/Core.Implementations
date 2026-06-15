namespace StruttonTechnologies.Core.Repositories.Crud;

public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{

    /// <summary>
    /// Asynchronously deletes an entity of type <typeparamref name="T"/> by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// The deleted entity if found and deleted; otherwise, <c>null</c> if the entity does not exist.
    /// </returns>
    public async Task<TEntity?> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        TEntity? entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        return await ExecuteWithExceptionHandlingAsync(async () =>
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync(cancellationToken);
            return entity;
        });
    }
}
