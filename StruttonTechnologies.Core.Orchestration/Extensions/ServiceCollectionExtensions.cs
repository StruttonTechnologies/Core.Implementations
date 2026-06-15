using Microsoft.Extensions.DependencyInjection;

using StruttonTechnologies.Core.Orchestration.Contracts;
using StruttonTechnologies.Core.Orchestration.Contracts.Crud;
using StruttonTechnologies.Core.Orchestration.Crud;
using StruttonTechnologies.Core.Orchestration.Services;

namespace StruttonTechnologies.Core.Orchestration;

/// <summary>
/// Provides extension methods for registering Core orchestration services.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreOrchestration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped(typeof(ICreateService<,>), typeof(CreateService<,>));
        services.AddScoped(typeof(IReadService<,>), typeof(ReadService<,>));
        services.AddScoped(typeof(IUpdateService<,>), typeof(UpdateService<,>));
        services.AddScoped(typeof(IDeleteService<,>), typeof(DeleteService<,>));
        services.AddScoped(typeof(ISoftDeleteService<,>), typeof(SoftDeleteService<,>));
        services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));
        services.AddScoped(typeof(IPersonOrchestration<>), typeof(PersonService<>));

        return services;
    }
}
