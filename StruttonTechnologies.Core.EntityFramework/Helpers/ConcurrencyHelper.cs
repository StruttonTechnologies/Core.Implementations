using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StruttonTechnologies.Core.Domain.Entities;

namespace StruttonTechnologies.Core.EntityFramework.Helpers;

public static class ConcurrencyHelper
{
    public static async Task HandleConcurrencyAsync<TUser>(DbUpdateConcurrencyException ex)
    {
        foreach (var entry in ex.Entries)
        {
            await entry.ReloadAsync();
        }
    }
}
