using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StruttonTechnologies.Core.Domain.Contracts;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Repositories.Crud;

/// <summary>
/// Provides a Guid-keyed specialization of <see cref="CrudRepository{TEntity, TKey}" />.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
public sealed class GuidCrudRepository<TEntity> : CrudRepository<TEntity, Guid>, ICrudRepository<TEntity>
    where TEntity : class, IEntity<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuidCrudRepository{TEntity}" /> class.
    /// </summary>
    /// <param name="context">The Entity Framework context used for stateful data access.</param>
    /// <param name="logger">The logger used for repository diagnostics.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is null.</exception>
    public GuidCrudRepository(
        DbContext context,
        ILogger<CrudRepository<TEntity, Guid>> logger)
        : base(context, logger)
    {
    }
}
