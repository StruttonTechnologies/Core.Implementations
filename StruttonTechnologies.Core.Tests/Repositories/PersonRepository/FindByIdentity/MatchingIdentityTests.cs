using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Repositories.Person;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.PersonRepository.FindByIdentity;

public class MatchingIdentityTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly PersonRepository<Person<int>, int> _repository;

    public MatchingIdentityTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new PersonRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_ReturnPerson_When_AllIdentityFieldsMatch()
    {
        var person = CreateTestPerson("John", "Doe", "john.doe@test.com");
        _context.Set<Person<int>>().Add(person);
        await _context.SaveChangesAsync();

        var result = await _repository.FindByIdentityAsync("John", "Doe", "john.doe@test.com");

        result.Should().NotBeNull();
        result!.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john.doe@test.com");
    }

    [Fact]
    public async Task Should_ReturnNull_When_FirstNameDoesNotMatch()
    {
        var person = CreateTestPerson("John", "Doe", "john.doe@test.com");
        _context.Set<Person<int>>().Add(person);
        await _context.SaveChangesAsync();

        var result = await _repository.FindByIdentityAsync("Jane", "Doe", "john.doe@test.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_ReturnNull_When_LastNameDoesNotMatch()
    {
        var person = CreateTestPerson("John", "Doe", "john.doe@test.com");
        _context.Set<Person<int>>().Add(person);
        await _context.SaveChangesAsync();

        var result = await _repository.FindByIdentityAsync("John", "Smith", "john.doe@test.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_ReturnNull_When_EmailDoesNotMatch()
    {
        var person = CreateTestPerson("John", "Doe", "john.doe@test.com");
        _context.Set<Person<int>>().Add(person);
        await _context.SaveChangesAsync();

        var result = await _repository.FindByIdentityAsync("John", "Doe", "jane.smith@test.com");

        result.Should().BeNull();
    }

    private static Person<int> CreateTestPerson(string firstName, string lastName, string email)
    {
        return new Person<int>
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            RowVersion = new byte[] { 1 }
        };
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
