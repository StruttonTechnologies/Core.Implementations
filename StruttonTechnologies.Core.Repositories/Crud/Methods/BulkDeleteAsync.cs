using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace StruttonTechnologies.Core.Repositories.Crud;

public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Deletes multiple entities in bulk (hard delete).
    /// </summary>
    /// <param name="ids">The IDs of the entities to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<IEnumerable<TEntity>> BulkDeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        // First, get all entities as a list, then filter in-memory using reflection
        List<TEntity> allEntities = await DbSet.ToListAsync(cancellationToken);
        var entitiesToDelete = allEntities
            .Where(e => ids.Contains((TKey)e.GetType().GetProperty("Id")!.GetValue(e)!))
            .ToList();

        // Detach any tracked entities with the same key to avoid tracking conflicts
        foreach (TEntity? entity in entitiesToDelete)
        {
            var entityId = (TKey)entity.GetType().GetProperty("Id")!.GetValue(entity)!;
            EntityEntry<TEntity>? tracked = Context.ChangeTracker.Entries<TEntity>()
                .FirstOrDefault(e => e.Entity != entity && EqualityComparer<TKey>.Default.Equals(
                    (TKey)e.Entity.GetType().GetProperty("Id")!.GetValue(e.Entity)!, entityId));
            tracked?.State = EntityState.Detached;
        }

        DbSet.RemoveRange(entitiesToDelete);
        return entitiesToDelete;
    }
}

