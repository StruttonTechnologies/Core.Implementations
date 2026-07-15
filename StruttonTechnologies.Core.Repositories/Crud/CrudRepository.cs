using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace StruttonTechnologies.Core.Repositories.Crud;

/// <summary>
/// Provides a generic repository implementation containing standard
/// create, read, update, delete, soft-delete, and query operations.
/// </summary>
/// <typeparam name="TEntity">
/// The entity type managed by the repository.
/// </typeparam>
/// <typeparam name="TKey">
/// The type of the entity's primary key.
/// </typeparam>
public partial class CrudRepository<TEntity, TKey> :
    RepositoryBase,
    ICrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets the entity set managed by the repository.
    /// </summary>
    protected DbSet<TEntity> DbSet { get; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="CrudRepository{TEntity, TKey}"/> class.
    /// </summary>
    /// <param name="context">
    /// The database context used by the repository.
    /// </param>
    /// <param name="logger">
    /// The logger used by the repository.
    /// </param>
    public CrudRepository(
        DbContext context,
        ILogger<CrudRepository<TEntity, TKey>> logger)
        : base(context, logger)
    {
        DbSet = context.Set<TEntity>();
    }
}
