using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StruttonTechnologies.Core.Repositories.Contracts.Base;
using StruttonTechnologies.Core.ToolKit.Exceptions;

namespace StruttonTechnologies.Core.Repositories.Base;

/// <summary>
/// Provides a base implementation for repositories, including
/// centralized exception handling for persistence operations.
/// 
/// Repositories stage changes but do not commit persistence.
/// </summary>
public abstract class RepositoryBase : IRepositoryBase
{
    protected DbContext Context { get; }
    protected ILogger Logger { get; }

    protected RepositoryBase(DbContext context, ILogger logger)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected async Task<TResult> ExecuteWithExceptionHandlingAsync<TResult>(
 Func<Task<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Logger.LogError(ex, "A concurrency conflict occurred during a persistence operation.");

            throw new ConcurrencyConflictException(
                "The record was modified by another user.",
                ex);
        }
        catch (DbUpdateException ex)
        {
            Logger.LogError(ex, "A database update error occurred during a persistence operation.");

            throw new PersistenceException(
                "A persistence error occurred while saving changes.",
                ex);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected persistence error occurred.");

            throw new PersistenceException(
                "An unexpected persistence error occurred.",
                ex);
        }
    }

    protected async Task<TResult> ExecuteQueryWithExceptionHandlingAsync<TResult>(Func<Task<TResult>> queryOperation)
    {
        try
        {
            return await queryOperation();
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogError(ex, "An invalid operation occurred during a query.");

            throw new PersistenceException(
                "An invalid operation occurred while executing a query.",
                ex);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected persistence error occurred during a query.");

            throw new PersistenceException(
                "An unexpected persistence error occurred while executing a query.",
                ex);
        }
    }
}
