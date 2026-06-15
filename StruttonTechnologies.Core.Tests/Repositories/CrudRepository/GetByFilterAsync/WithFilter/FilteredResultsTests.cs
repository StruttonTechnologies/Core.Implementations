using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.CrudRepository.GetByFilterAsync.WithFilter;

public class FilteredResultsTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly CrudRepository<Person<int>, int> _repository;

    public FilteredResultsTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new CrudRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_ReturnMatchingEntities_When_FilterIsApplied()
    {
        var person1 = CreateTestPerson("John", "Doe", "john@test.com");
        var person2 = CreateTestPerson("Jane", "Doe", "jane@test.com");
        var person3 = CreateTestPerson("Bob", "Smith", "bob@test.com");

        _context.Set<Person<int>>().AddRange(person1, person2, person3);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByFilterAsync<string>(
            filter: p => p.LastName == "Doe");

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.LastName.Should().Be("Doe"));
    }

    [Fact]
    public async Task Should_ReturnEmpty_When_NoEntitiesMatchFilter()
    {
        var person1 = CreateTestPerson("John", "Doe", "john@test.com");
        var person2 = CreateTestPerson("Jane", "Smith", "jane@test.com");

        _context.Set<Person<int>>().AddRange(person1, person2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByFilterAsync<string>(
            filter: p => p.LastName == "NonExistent");

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_ReturnAllEntities_When_FilterIsNull()
    {
        var person1 = CreateTestPerson("John", "Doe", "john@test.com");
        var person2 = CreateTestPerson("Jane", "Smith", "jane@test.com");

        _context.Set<Person<int>>().AddRange(person1, person2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByFilterAsync<string>(filter: null);

        result.Should().HaveCount(2);
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
