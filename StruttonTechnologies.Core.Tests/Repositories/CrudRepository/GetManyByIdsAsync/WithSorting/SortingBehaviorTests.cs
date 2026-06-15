using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.CrudRepository.GetManyByIdsAsync.WithSorting;

public class SortingBehaviorTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly CrudRepository<Person<int>, int> _repository;

    public SortingBehaviorTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new CrudRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_SortResultsAscending_When_SortingIsEnabled()
    {
        var person1 = CreateTestPerson("Charlie", "Doe", "charlie@test.com");
        var person2 = CreateTestPerson("Alice", "Smith", "alice@test.com");
        var person3 = CreateTestPerson("Bob", "Johnson", "bob@test.com");

        _context.Set<Person<int>>().AddRange(person1, person2, person3);
        await _context.SaveChangesAsync();

        var ids = new[] { person1.Id, person2.Id, person3.Id };
        var result = await _repository.GetManyByIdsAsync(
            ids: ids,
            isSorted: true,
            orderBy: p => p.FirstName,
            ascending: true);

        var list = result.ToList();
        list[0].FirstName.Should().Be("Alice");
        list[1].FirstName.Should().Be("Bob");
        list[2].FirstName.Should().Be("Charlie");
    }

    [Fact]
    public async Task Should_SortResultsDescending_When_DescendingSpecified()
    {
        var person1 = CreateTestPerson("Alice", "Doe", "alice@test.com");
        var person2 = CreateTestPerson("Bob", "Smith", "bob@test.com");
        var person3 = CreateTestPerson("Charlie", "Johnson", "charlie@test.com");

        _context.Set<Person<int>>().AddRange(person1, person2, person3);
        await _context.SaveChangesAsync();

        var ids = new[] { person1.Id, person2.Id, person3.Id };
        var result = await _repository.GetManyByIdsAsync(
            ids: ids,
            isSorted: true,
            orderBy: p => p.FirstName,
            ascending: false);

        var list = result.ToList();
        list[0].FirstName.Should().Be("Charlie");
        list[1].FirstName.Should().Be("Bob");
        list[2].FirstName.Should().Be("Alice");
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
