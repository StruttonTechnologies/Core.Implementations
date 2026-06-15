using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using StruttonTechnologies.Core.Domain.Entities.Base;

namespace StruttonTechnologies.Core.EntityFramework.Configurations;

public static class EntityBaseConfiguration
{
    public static void ApplyEntityBaseRules(this DbContext context)
    {
        DateTime now = DateTime.UtcNow;

        foreach (EntityEntry<EntityBase<Guid>> entry in context.ChangeTracker.Entries<EntityBase<Guid>>())
        {
            EntityBase<Guid> entity = entry.Entity;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreatedAt = now;
                    // entity.CreatedBy ??= "system"; 
                    break;

                case EntityState.Modified:
                    entity.ModifiedAt = now;
                    // entity.ModifiedBy ??= "system";
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entity.IsDeleted = true;
                    entity.DeletedAt = now;
                    break;
            }
        }
    }
}
