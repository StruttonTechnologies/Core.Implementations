using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StruttonTechnologies.Core.EF.Contracts;
using StruttonTechnologies.Core.EntityFramework.Interceptors;

namespace StruttonTechnologies.Core.EntityFramework.Extensions;

/// <summary>
/// Provides dependency injection extensions for registering
/// Strutton Technologies Core Entity Framework services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Core Entity Framework services, the application DbContext,
    /// and its contract mappings.
    /// </summary>
    /// <typeparam name="TDbContext">The application DbContext type.</typeparam>
    /// <typeparam name="TDbContextContract">The application DbContext contract type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">
    /// A delegate used to configure the DbContext provider and options.
    /// </param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCoreEntityFramework<TDbContext, TDbContextContract, TKey>(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> configureOptions)
        where TDbContext : DbContext, TDbContextContract, IUnitOfWork
        where TDbContextContract : class
        where TKey : IEquatable<TKey>
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.AddScoped<AuditInterceptor<TKey>>();
        services.AddScoped<SoftDeleteInterceptor<Guid>>();

        services.AddDbContext<TDbContext>((serviceProvider, options) =>
        {
            configureOptions(serviceProvider, options);

            options.AddInterceptors(
                serviceProvider.GetRequiredService<AuditInterceptor<TKey>>(),
                serviceProvider.GetRequiredService<SoftDeleteInterceptor<Guid>>());
        });

        services.AddScoped<TDbContextContract>(serviceProvider =>
            serviceProvider.GetRequiredService<TDbContext>());

        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<TDbContext>());

        return services;
    }
}
