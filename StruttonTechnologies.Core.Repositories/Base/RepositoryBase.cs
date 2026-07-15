using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StruttonTechnologies.Core.ToolKit.Exceptions;

namespace StruttonTechnologies.Core.Repositories.Base;

/// <summary>
/// Provides shared infrastructure for repositories, including access to
/// the database context, logging, and centralized persistence exception handling.
/// </summary>
/// <remarks>
/// Repositories stage changes in the current unit of work but do not commit
/// those changes. The owning transaction boundary is responsible for calling
/// <see cref="DbContext.SaveChangesAsync(CancellationToken)"/>.
/// </remarks>
public abstract class RepositoryBase
{
    /// <summary>
    /// Gets the database context used by the repository.
    /// </summary>
    protected DbContext Context { get; }

    /// <summary>
    /// Gets the logger used by the repository.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The repository logger.</param>
    protected RepositoryBase(
        DbContext context,
        ILogger logger)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes a persistence operation and translates infrastructure exceptions
    /// into application-specific persistence exceptions.
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="operation">The persistence operation to execute.</param>
    /// <returns>The result returned by the operation.</returns>
    /// <exception cref="ConcurrencyConflictException">
    /// Thrown when an optimistic concurrency conflict occurs.
    /// </exception>
    /// <exception cref="PersistenceException">
    /// Thrown when a persistence operation fails.
    /// </exception>
    protected async Task<TResult> ExecuteWithExceptionHandlingAsync<TResult>(
        Func<Task<TResult>> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        try
        {
            return await operation();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Logger.LogError(
                ex,
                "A concurrency conflict occurred during a persistence operation.");

            throw new ConcurrencyConflictException(
                "The record was modified by another user.",
                ex);
        }
        catch (DbUpdateException ex)
        {
            Logger.LogError(
                ex,
                "A database update error occurred during a persistence operation.");

            throw new PersistenceException(
                "A persistence error occurred while staging database changes.",
                ex);
        }
        catch (Exception ex) when (ex is not PersistenceException)
        {
            Logger.LogError(
                ex,
                "An unexpected persistence error occurred.");

            throw new PersistenceException(
                "An unexpected persistence error occurred.",
                ex);
        }
    }

    /// <summary>
    /// Executes a query operation and translates infrastructure exceptions
    /// into application-specific persistence exceptions.
    /// </summary>
    /// <typeparam name="TResult">The query result type.</typeparam>
    /// <param name="queryOperation">The query operation to execute.</param>
    /// <returns>The result returned by the query.</returns>
    /// <exception cref="PersistenceException">
    /// Thrown when the query operation fails.
    /// </exception>
    protected async Task<TResult> ExecuteQueryWithExceptionHandlingAsync<TResult>(
        Func<Task<TResult>> queryOperation)
    {
        ArgumentNullException.ThrowIfNull(queryOperation);

        try
        {
            return await queryOperation();
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogError(
                ex,
                "An invalid operation occurred during a query.");

            throw new PersistenceException(
                "An invalid operation occurred while executing a query.",
                ex);
        }
        catch (Exception ex) when (ex is not PersistenceException)
        {
            Logger.LogError(
                ex,
                "An unexpected persistence error occurred during a query.");

            throw new PersistenceException(
                "An unexpected persistence error occurred while executing a query.",
                ex);
        }
    }
}
