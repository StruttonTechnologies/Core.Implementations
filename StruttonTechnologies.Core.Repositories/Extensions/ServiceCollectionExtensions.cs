using Microsoft.Extensions.DependencyInjection;

using StruttonTechnologies.Core.Repositories.Contracts;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Repositories.Person;

namespace StruttonTechnologies.Core.Repositories;

/// <summary>
/// Provides extension methods for registering Core repository services.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreRepositories(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

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

        services.AddScoped(typeof(IPersonRepository<,>), typeof(PersonRepository<,>));

        return services;
    }
}
