using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using StruttonTechnologies.Core.Domain.Entities.Base;
using StruttonTechnologies.Core.EF.Contracts;

namespace StruttonTechnologies.Core.EntityFramework.Interceptors;

/// <summary>
/// Intercepts save operations and applies audit values
/// to entities derived from <see cref="EntityBase{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">
/// The key type used by the application for entity and identity identifiers.
/// </typeparam>
public sealed class AuditInterceptor<TKey> : SaveChangesInterceptor
    where TKey : IEquatable<TKey>
{
    private const string CreatedAtPropertyName = nameof(EntityBase<int>.CreatedAt);
    private const string CreatedByPropertyName = nameof(EntityBase<int>.CreatedBy);
    private const string ModifiedAtPropertyName = nameof(EntityBase<int>.ModifiedAt);
    private const string ModifiedByPropertyName = nameof(EntityBase<int>.ModifiedBy);

    private readonly ICurrentUserProvider<TKey> _currentUserProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditInterceptor{TKey}"/> class.
    /// </summary>
    /// <param name="currentUserProvider">The current user provider.</param>
    public AuditInterceptor(ICurrentUserProvider<TKey> currentUserProvider)
    {
        _currentUserProvider = currentUserProvider;
    }

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ApplyAuditValues(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ApplyAuditValues(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Applies audit values and protects creation metadata
    /// for tracked entities derived from <see cref="EntityBase{TKey}"/>.
    /// </summary>
    /// <param name="dbContext">The current database context.</param>
    private void ApplyAuditValues(DbContext dbContext)
    {
        DateTime utcNow = DateTime.UtcNow;
        TKey? currentUserId = _currentUserProvider.GetCurrentUserId();

        foreach (EntityEntry entry in dbContext.ChangeTracker.Entries())
        {
            if (!IsEntityBaseType(entry.Entity.GetType()))
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.CurrentValues[CreatedAtPropertyName] = utcNow;

                    if (!IsDefaultValue(currentUserId))
                    {
                        entry.CurrentValues[CreatedByPropertyName] = currentUserId;
                    }

                    break;

                case EntityState.Modified:
                    entry.CurrentValues[ModifiedAtPropertyName] = utcNow;

                    if (!IsDefaultValue(currentUserId))
                    {
                        entry.CurrentValues[ModifiedByPropertyName] = currentUserId;
                    }

                    entry.Property(CreatedAtPropertyName).IsModified = false;
                    entry.Property(CreatedByPropertyName).IsModified = false;
                    break;
            }
        }
    }

    /// <summary>
    /// Determines whether the specified type derives from <see cref="EntityBase{TKey}"/>.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>
    /// <see langword="true"/> if the type derives from <see cref="EntityBase{TKey}"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    private static bool IsEntityBaseType(Type type)
    {
        while (type is not null)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(EntityBase<>))
            {
                return true;
            }

            type = type.BaseType!;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified value is the default value for <typeparamref name="TKey"/>.
    /// </summary>
    /// <param name="value">The value to inspect.</param>
    /// <returns>
    /// <see langword="true"/> if the value is the default value for <typeparamref name="TKey"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    private static bool IsDefaultValue(TKey? value)
    {
        return EqualityComparer<TKey?>.Default.Equals(value, default);
    }
}
