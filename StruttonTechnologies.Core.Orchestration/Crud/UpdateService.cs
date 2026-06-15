using StruttonTechnologies.Core.Orchestration.Contracts.Crud;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Orchestration.Crud;

/// <summary>
/// Service for updating entities via the provided repository.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TKey">Entity key type.</typeparam>
public class UpdateService<TEntity, TKey>(
    ICrudRepository<TEntity, TKey> repository)
    : IUpdateService<TEntity, TKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Updates a single entity.
    /// </summary>
    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return repository.UpdateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Updates multiple entities in bulk.
    /// </summary>
    public Task<IEnumerable<TEntity>> BulkUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return repository.BulkUpdateAsync(entities, cancellationToken);
    }
}
