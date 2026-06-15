using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Contracts;
using StruttonTechnologies.Core.Repositories.Crud;


namespace StruttonTechnologies.Core.Repositories.Person;

/// <summary>
/// Repository for managing Person entities with audit and transaction support.
/// </summary>
/// <typeparam name="TPerson">The concrete person type.</typeparam>
/// <typeparam name="TKey">The type of the person identifier.</typeparam>
public class PersonRepository<TPerson, TKey>(
    DbContext context,
    ILogger<CrudRepository<TPerson, TKey>> logger)
    : CrudRepository<TPerson, TKey>(context, logger), IPersonRepository<TPerson, TKey>
    where TPerson : Person<TKey>, new()
    where TKey : IEquatable<TKey>
{
    /// <inheritdoc />
    public async Task<TPerson?> FindByIdentityAsync(
        string firstName,
        string lastName,
        string Email,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(
                p => p.FirstName == firstName
                  && p.LastName == lastName
                  && p.Email == Email,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TPerson?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(
            p => p.Email == email,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(p => p.Email == email, cancellationToken);
    }
}
