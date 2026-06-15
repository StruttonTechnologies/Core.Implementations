using Microsoft.Extensions.DependencyInjection;

using StruttonTechnologies.Core.Repositories.Crud;

namespace StruttonTechnologies.Core.Repositories;

/// <summary>
/// Provides service registrations for repository services.
/// </summary>
public class RepositoryRegistrar
{
    /// <summary>
    /// Creates and returns the repository service registrations.
    /// </summary>
    /// <returns>The service collection containing repository registrations.</returns>
    public IServiceCollection GetRegistrar()
    {
        ServiceCollection services = new ServiceCollection();

        services.AddScoped(typeof(ICrudRepository<,>), typeof(CrudRepository<,>));
        services.AddScoped(typeof(IReadRepository<,>), typeof(CrudRepository<,>));
        services.AddScoped(typeof(IDeleteRepository<,>), typeof(CrudRepository<,>));
        services.AddScoped(typeof(ISoftDeleteRepository<,>), typeof(CrudRepository<,>));
        services.AddScoped(typeof(IQueryHelpers<,>), typeof(CrudRepository<,>));

        services.AddScoped(typeof(ICrudRepository<>), typeof(GuidCrudRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(GuidCrudRepository<>));
        services.AddScoped(typeof(ICreateRepository<>), typeof(GuidCrudRepository<>));
        services.AddScoped(typeof(IUpdateRepository<>), typeof(GuidCrudRepository<>));
        services.AddScoped(typeof(IDeleteRepository<>), typeof(GuidCrudRepository<>));
        services.AddScoped(typeof(ISoftDeleteRepository<>), typeof(GuidCrudRepository<>));

        return services;
    }
}
