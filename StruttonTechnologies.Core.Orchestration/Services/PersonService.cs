using Microsoft.Extensions.Logging;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Orchestration.Contracts;
using StruttonTechnologies.Core.Repositories.Contracts;

namespace StruttonTechnologies.Core.Orchestration.Services;

public class PersonService<TKey>(
    IPersonRepository<Person<TKey>, TKey> repository,
    ILogger<PersonService<TKey>> logger) : IPersonOrchestration<TKey>
    where TKey : IEquatable<TKey>
{
    public async Task<Person<TKey>?> CreatePersonAsync(Person<TKey> person, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to create person with email {Email}", person.Email);

        await repository.AddAsync(person, cancellationToken);

        logger.LogInformation("Successfully created person {Id}", person.Id);

        return person;
    }

    public async Task<Person<TKey>?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await repository.GetByEmailAsync(email, cancellationToken);
    }

    public async Task<Person<TKey>?> FindByIdentityAsync(string firstName, string lastName, string Email, CancellationToken cancellationToken = default)
    {
        return await repository.FindByIdentityAsync(firstName, lastName, Email, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return repository.EmailExistsAsync(email, cancellationToken);
    }
}
