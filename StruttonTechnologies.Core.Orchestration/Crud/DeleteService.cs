using Microsoft.Extensions.Logging;
using StruttonTechnologies.Core.Orchestration.Contracts.Crud;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Orchestration.Crud;

public class DeleteService<TEntity, TKey>(
    ICrudRepository<TEntity, TKey> repository,
    ILogger<DeleteService<TEntity, TKey>> logger) : IDeleteService<TEntity, TKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    public virtual Task<TEntity?> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Deleting entity {Id} of type {EntityType}", id, typeof(TEntity).Name);
        return repository.DeleteAsync(id, cancellationToken);
    }

    public virtual Task<IEnumerable<TEntity>> BulkDeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Bulk deleting entities of type {EntityType}", typeof(TEntity).Name);
        return repository.BulkDeleteAsync(ids, cancellationToken);
    }
}
