using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Domain.Entities.Audit;
using StruttonTechnologies.Core.Domain.Entities.Base;
using StruttonTechnologies.Core.EF.Contracts;
using StruttonTechnologies.Core.Identity.Domain.Entities;
using StruttonTechnologies.Core.Identity.EF;
using StruttonTechnologies.Core.ToolKit.Exceptions;

namespace StruttonTechnologies.Core.EntityFramework.Context;

/// <summary>
/// Generic convenience base <see cref="DbContext"/> that assumes a <see cref="Guid"/>
/// key for person and identity types.
/// </summary>
/// <typeparam name="TPerson">The person entity type.</typeparam>
/// <typeparam name="TUser">The identity user type.</typeparam>
/// <typeparam name="TRole">The identity role type.</typeparam>
/// <param name="options">The options used to configure the context.</param>
public abstract class CoreDbContext<TPerson, TUser, TRole>(DbContextOptions options)
    : CoreDbContext<TPerson, Guid, TUser, TRole>(options)
    where TPerson : Person<Guid>
    where TUser : IdentityUser<Guid>
    where TRole : IdentityRole<Guid>
{
}

/// <summary>
/// Provides the core application <see cref="DbContext"/> implementation with shared
/// entity sets, global soft-delete filtering, and unit of work support.
/// </summary>
/// <typeparam name="TPerson">The person entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
/// <typeparam name="TUser">The identity user type.</typeparam>
/// <typeparam name="TRole">The identity role type.</typeparam>
/// <param name="options">The options used to configure the context.</param>
public abstract class CoreDbContext<TPerson, TKey, TUser, TRole>(DbContextOptions options)
    : CoreIdentityDbContext<TKey, TUser, TRole>(options), IUnitOfWork
    where TPerson : Person<TKey>
    where TKey : IEquatable<TKey>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
{
    /// <summary>
    /// Gets or sets the people set.
    /// </summary>
    public DbSet<TPerson> People { get; set; } = null!;

    /// <summary>
    /// Gets or sets the audit log entries set.
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    /// <summary>
    /// Configures the model for the context.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CoreDbContext<TPerson, TKey, TUser, TRole>).Assembly);

        ApplySoftDeleteFilters(modelBuilder);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyConflictException(
                "The record was modified by another user.",
                ex);
        }
        catch (DbUpdateException ex)
        {
            throw new PersistenceException(
                "A persistence error occurred while saving changes.",
                ex);
        }
        catch (Exception ex)
        {
            throw new PersistenceException(
                "An unexpected persistence error occurred while saving changes.",
                ex);
        }
    }

    /// <summary>
    /// Applies global soft-delete query filters to all entity types that derive from
    /// <see cref="EntityBase{TKey}"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            Type clrType = entityType.ClrType;

            if (!typeof(EntityBase<TKey>).IsAssignableFrom(clrType))
            {
                continue;
            }

            MethodInfo method = typeof(CoreDbContext<TPerson, TKey, TUser, TRole>)
                .GetMethod(
                    nameof(SetSoftDeleteFilter),
                    BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(clrType);

            method.Invoke(null, new object[] { modelBuilder });
        }
    }

    /// <summary>
    /// Applies a global query filter that excludes soft-deleted rows for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to configure.</typeparam>
    /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : EntityBase<TKey>
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(entity => !entity.IsDeleted);
    }
}
