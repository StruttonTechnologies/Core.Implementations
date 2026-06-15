using Microsoft.EntityFrameworkCore;

namespace StruttonTechnologies.Core.EntityFramework.Utilities;

/// <summary>
/// Provides helper methods for executing database operations within an
/// explicit Entity Framework transaction.
/// </summary>
public static class EfTransactionUtilities
{
    /// <summary>
    /// Executes the specified asynchronous action within a database transaction.
    /// </summary>
    /// <param name="dbContext">
    /// The <see cref="DbContext"/> used to create and manage the transaction.
    /// </param>
    /// <param name="action">
    /// The asynchronous action to execute within the transaction scope.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous transactional operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dbContext"/> or <paramref name="action"/> is null.
    /// </exception>
    public static async Task ExecuteTransactionAsync(
        DbContext dbContext,
        Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(action);

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}