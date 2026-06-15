namespace StruttonTechnologies.Core.EntityFramework.Extensions;

/// <summary>
/// Provides common query extensions for Entity Framework queries.
/// </summary>
public static class EfQueryExtensions
{
    /// <summary>
    /// Applies paging to the query.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="page">The 1-based page number.</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <returns>A query limited to the requested page.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="query"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="page"/> or <paramref name="pageSize"/> is less than 1.
    /// </exception>
    public static IQueryable<TEntity> Paginate<TEntity>(
        this IQueryable<TEntity> query,
        int page,
        int pageSize)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(query);

        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page));
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize));
        }

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}