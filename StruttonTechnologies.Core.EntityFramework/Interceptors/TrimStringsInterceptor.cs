using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace StruttonTechnologies.Core.EntityFramework.Interceptors;

public class TrimStringsInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        TrimStrings(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public static void TrimStrings(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            var props = entry.CurrentValues.Properties
                .Where(p => p.ClrType == typeof(string));

            foreach (var prop in props)
            {
                var current = entry.CurrentValues[prop.Name] as string;
                if (current != null)
                {
                    entry.CurrentValues[prop.Name] = current.Trim();
                }
            }
        }
    }
}
