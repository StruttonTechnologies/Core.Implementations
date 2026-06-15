using Microsoft.EntityFrameworkCore;
using StruttonTechnologies.Core.Domain.Entities.Base;

namespace StruttonTechnologies.Core.EntityFramework.Extensions;

/// <summary>
/// Provides query extensions for working with soft-deleted entities.
/// </summary>
public static class SoftDeleteQueryExtensions
{
    /// <summary>
    /// Returns a query that includes both active and soft-deleted entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The source query.</param>
    /// <returns>
    /// A query that ignores the global soft-delete filter and includes all matching entities.
    /// </returns>
    public static IQueryable<TEntity> IncludeDeleted<TEntity>(this IQueryable<TEntity> query)
        where TEntity : class, ISoftDeletable
    {
        ArgumentNullException.ThrowIfNull(query);

        return query.IgnoreQueryFilters();
    }

    /// <summary>
    /// Returns a query that includes only soft-deleted entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The source query.</param>
    /// <returns>
    /// A query that ignores the global soft-delete filter and returns only soft-deleted entities.
    /// </returns>
    public static IQueryable<TEntity> OnlyDeleted<TEntity>(this IQueryable<TEntity> query)
        where TEntity : class, ISoftDeletable
    {
        ArgumentNullException.ThrowIfNull(query);

        return query
            .IgnoreQueryFilters()
            .Where(entity => entity.IsDeleted);
    }

    /// <summary>
    /// Returns a query that includes only active, non-deleted entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The source query.</param>
    /// <returns>
    /// A query that returns only active, non-deleted entities.
    /// </returns>
    public static IQueryable<TEntity> ActiveOnly<TEntity>(this IQueryable<TEntity> query)
        where TEntity : class, ISoftDeletable
    {
        ArgumentNullException.ThrowIfNull(query);

        return query
            .IgnoreQueryFilters()
            .Where(entity => !entity.IsDeleted);
    }
}