using Microsoft.Extensions.DependencyInjection;

using StruttonTechnologies.Core.Orchestration.Contracts;
using StruttonTechnologies.Core.Orchestration.Contracts.Crud;
using StruttonTechnologies.Core.Orchestration.Crud;
using StruttonTechnologies.Core.Orchestration.Services;
using StruttonTechnologies.Core.Repositories;
using StruttonTechnologies.Core.ToolKit.Registration.Utilities;

namespace StruttonTechnologies.Core.Orchestration;

/// <summary>
/// Provides service registrations for orchestration services.
/// </summary>
/// <param name="repositoryRegistrar">The registrar used to compose repository registrations.</param>
public class OrchestrationRegistrar(RepositoryRegistrar repositoryRegistrar)
{
    private readonly RepositoryRegistrar _repositoryRegistrar = repositoryRegistrar
        ?? throw new ArgumentNullException(nameof(repositoryRegistrar));

    /// <summary>
    /// Creates and returns the orchestration service registrations.
    /// </summary>
    /// <returns>The service collection containing orchestration registrations.</returns>
    public IServiceCollection GetRegistrar()
    {
        ServiceCollection services = new ServiceCollection();
        IServiceCollection repositoryServices = _repositoryRegistrar.GetRegistrar();

        ServiceCollectionComposer.Compose(services, repositoryServices);

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
