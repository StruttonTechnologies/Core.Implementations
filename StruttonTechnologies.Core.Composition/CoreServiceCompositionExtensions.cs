using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StruttonTechnologies.Core.API.Extensions;
using StruttonTechnologies.Core.Coordinator;
using StruttonTechnologies.Core.EF.Contracts;
using StruttonTechnologies.Core.EntityFramework.Extensions;
using StruttonTechnologies.Core.Identity.Composition;
using StruttonTechnologies.Core.Identity.EF;
using StruttonTechnologies.Core.Orchestration;
using StruttonTechnologies.Core.Repositories;

namespace StruttonTechnologies.Core.Composition;

/// <summary>
/// Single public composition facade for Strutton Technologies Core services.
/// </summary>
public static class CoreServiceCompositionExtensions
{
    /// <summary>
    /// Registers provider-neutral Core services.
    /// </summary>
    public static IServiceCollection AddStruttonTechnologiesCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddCoreApi();
        services.AddCoreRepositories();
        services.AddCoreOrchestration();
        services.AddCoreCoordinator();
        //services.AddCoreUi();

        return services;
    }

    /// <summary>
    /// Registers provider-neutral Core services and Core Entity Framework services.
    /// The consuming application still owns the provider/connection-string decision.
    /// </summary>
    public static IServiceCollection AddStruttonTechnologiesCore<TDbContext, TDbContextContract, TKey>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IServiceProvider, DbContextOptionsBuilder> configureOptions)
        where TDbContext : DbContext, TDbContextContract, IUnitOfWork
        where TDbContextContract : class
        where TKey : IEquatable<TKey>
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.AddStruttonTechnologiesCore(configuration);
        services.AddCoreEntityFramework<TDbContext, TDbContextContract, TKey>(configureOptions);

        return services;
    }

    /// <summary>
    /// Registers provider-neutral Core services and Core Identity services.
    /// The consuming application must register the DbContext provider before calling this method.
    /// </summary>
    public static IServiceCollection AddStruttonTechnologiesCore<TContext, TUser, TRole, TKey>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TContext : CoreIdentityDbContext<TKey, TUser, TRole>
        where TUser : Identity.Domain.Entities.IdentityUser<TKey>, new()
        where TRole : Identity.Domain.Entities.IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddStruttonTechnologiesCore(configuration);
        services.AddCoreIdentity<TContext, TUser, TRole, TKey>(configuration);

        return services;
    }

    /// <summary>
    /// Registers provider-neutral Core services and Core Identity services using string keys.
    /// </summary>
    public static IServiceCollection AddStruttonTechnologiesCore<TContext, TUser, TRole>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TContext : CoreIdentityDbContext<string, TUser, TRole>
        where TUser : Identity.Domain.Entities.IdentityUser<string>, new()
        where TRole : Identity.Domain.Entities.IdentityRole<string>, new()
    {
        return services.AddStruttonTechnologiesCore<TContext, TUser, TRole, string>(configuration);
    }
}
