using Microsoft.EntityFrameworkCore;

namespace StruttonTechnologies.Core.EntityFramework.Utilities;

/// <summary>
/// Provides helper methods for handling optimistic concurrency conflicts in Entity Framework Core.
/// </summary>
public static class EfConcurrencyUtilities
{
    /// <summary>
    /// Executes an operation and retries if a concurrency conflict occurs.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="maxRetries">The maximum number of retry attempts.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task ExecuteWithConcurrencyRetryAsync(
        Func<Task> operation,
        int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));

        var retryCount = 0;

        while (true)
        {
            try
            {
                await operation();
                return;
            }
            catch (DbUpdateConcurrencyException)
            {
                retryCount++;

                if (retryCount > maxRetries)
                    throw;
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    /// <summary>
    /// Reloads entities involved in a concurrency conflict.
    /// </summary>
    /// <param name="exception">The concurrency exception.</param>
    public static async Task ReloadConflictedEntriesAsync(DbUpdateConcurrencyException exception)
    {
        foreach (var entry in exception.Entries)
        {
            await entry.ReloadAsync();
        }
    }
}