using Microsoft.Extensions.DependencyInjection;

using StruttonTechnologies.Core.Coordinator.Contracts;

namespace StruttonTechnologies.Core.Coordinator;

/// <summary>
/// Provides extension methods for registering Core coordinator services and handlers.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreCoordinator(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ICoordinator, Coordinator>();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        return services;
    }
}
