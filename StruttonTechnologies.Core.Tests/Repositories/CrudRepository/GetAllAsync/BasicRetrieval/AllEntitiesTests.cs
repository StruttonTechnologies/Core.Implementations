using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.CrudRepository.GetAllAsync.BasicRetrieval;

public class AllEntitiesTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly CrudRepository<Person<int>, int> _repository;

    public AllEntitiesTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new CrudRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_ReturnAllEntities_When_NoFilterApplied()
    {
        var person1 = CreateTestPerson("John", "Doe", "john@test.com");
        var person2 = CreateTestPerson("Jane", "Smith", "jane@test.com");
        var person3 = CreateTestPerson("Bob", "Johnson", "bob@test.com");

        _context.Set<Person<int>>().AddRange(person1, person2, person3);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Email == "john@test.com");
        result.Should().Contain(p => p.Email == "jane@test.com");
        result.Should().Contain(p => p.Email == "bob@test.com");
    }

    [Fact]
    public async Task Should_ReturnEmptyCollection_When_NoEntitiesExist()
    {
        var result = await _repository.GetAllAsync();

        result.Should().BeEmpty();
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
