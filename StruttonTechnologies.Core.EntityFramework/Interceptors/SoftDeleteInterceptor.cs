using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using StruttonTechnologies.Core.Domain.Entities.Base;
using StruttonTechnologies.Core.EF.Contracts;

namespace StruttonTechnologies.Core.EntityFramework.Interceptors;

/// <summary>
/// Intercepts delete operations for entities derived from <see cref="EntityBase{TKey}"/>
/// and converts them into soft-delete updates.
/// </summary>
/// <typeparam name="TKey">
/// The key type used by the application for entity and identity identifiers.
/// </typeparam>
public sealed class SoftDeleteInterceptor<TKey> : SaveChangesInterceptor
    where TKey : IEquatable<TKey>
{
    private const string IsDeletedPropertyName = nameof(EntityBase<int>.IsDeleted);
    private const string DeletedAtPropertyName = nameof(EntityBase<int>.DeletedAt);
    private const string DeletedByPropertyName = nameof(EntityBase<int>.DeletedBy);

    private readonly ICurrentUserProvider<TKey> _currentUserProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoftDeleteInterceptor{TKey}"/> class.
    /// </summary>
    /// <param name="currentUserProvider">The current user provider.</param>
    public SoftDeleteInterceptor(ICurrentUserProvider<TKey> currentUserProvider)
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
            ApplySoftDelete(eventData.Context);
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
            ApplySoftDelete(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Converts delete operations on tracked entities derived from <see cref="EntityBase{TKey}"/>
    /// into soft-delete updates.
    /// </summary>
    /// <param name="dbContext">The current database context.</param>
    private void ApplySoftDelete(DbContext dbContext)
    {
        DateTime utcNow = DateTime.UtcNow;
        TKey? currentUserId = _currentUserProvider.GetCurrentUserId();

        foreach (EntityEntry entry in dbContext.ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            if (!IsEntityBaseType(entry.Entity.GetType()))
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entry.CurrentValues[IsDeletedPropertyName] = true;
            entry.CurrentValues[DeletedAtPropertyName] = utcNow;

            if (!IsDefaultValue(currentUserId))
            {
                entry.CurrentValues[DeletedByPropertyName] = currentUserId;
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
