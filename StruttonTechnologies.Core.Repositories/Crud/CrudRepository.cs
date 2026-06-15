using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StruttonTechnologies.Core.Domain.Contracts;
using StruttonTechnologies.Core.Repositories.Base;
using StruttonTechnologies.Core.Repositories.Contracts.Base;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Repositories.Crud;

/// <summary>
/// A generic repository class that provides standard CRUD operations for entities.
/// This class can be used for any entity type that is a class and any DbContext.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
public partial class CrudRepository<TEntity, TKey>
    : RepositoryBase,
      IRepositoryBase,
      ICrudRepository<TEntity, TKey>,
      IReadRepository<TEntity, TKey>,
      ICreateRepository<TEntity>,
      IUpdateRepository<TEntity>,
      IDeleteRepository<TEntity, TKey>,
      ISoftDeleteRepository<TEntity, TKey>,
      IQueryHelpers<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    protected DbSet<TEntity> DbSet { get; }
    protected new ILogger<CrudRepository<TEntity, TKey>> Logger { get; }

    public CrudRepository(
        DbContext context,
        ILogger<CrudRepository<TEntity, TKey>> logger)
        : base(context, logger)
    {
        DbSet = context.Set<TEntity>()
            ?? throw new InvalidOperationException(
                $"DbSet<{typeof(TEntity).Name}> could not be resolved. Ensure it's registered in OnModelCreating.");

        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
