using Microsoft.Extensions.Logging;

using StruttonTechnologies.Core.Domain.Contracts;
using StruttonTechnologies.Core.Orchestration.Contracts.Crud;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Orchestration.Crud;

public class SoftDeleteService<TEntity, TKey>(
    ICrudRepository<TEntity, TKey> repository,
    ILogger<SoftDeleteService<TEntity, TKey>> logger) :
    ISoftDeleteService<TEntity, TKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    public virtual async Task<TEntity?> SoftDeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Soft deleting entity {Id} of type {EntityType}", id, typeof(TEntity).Name);

        TEntity? entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            logger.LogWarning("Soft delete skipped; entity {Id} not found", id);
            return null;
        }

        if (entity is not ISoftDeletable soft)
        {
            throw new NotSupportedException(
                $"Entity type {typeof(TEntity).Name} does not implement ISoftDeletable and cannot be soft deleted.");
        }

        soft.IsDeleted = true;
        soft.DeletedDate = DateTime.UtcNow;

        TEntity updated = await repository.UpdateAsync(entity, cancellationToken);
        logger.LogInformation("Soft deleted entity {Id} of type {EntityType}", id, typeof(TEntity).Name);
        return updated;
    }

    public virtual async Task<IEnumerable<TEntity>> BulkSoftDeleteAsync(
        IEnumerable<TKey> ids,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Bulk soft deleting entities of type {EntityType}", typeof(TEntity).Name);

        List<TEntity> updatedEntities = new List<TEntity>();
        foreach (TKey id in ids)
        {
            TEntity? updated = await SoftDeleteAsync(id, cancellationToken);
            if (updated is not null)
            {
                updatedEntities.Add(updated);
            }
        }

        logger.LogInformation("Bulk soft delete completed for {Count} entities of type {EntityType}",
            updatedEntities.Count, typeof(TEntity).Name);

        return updatedEntities;
    }
}
