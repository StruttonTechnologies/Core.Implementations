using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.CrudRepository.GetAllAsync.Sorting;

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
    public async Task Should_SortAscending_When_AscendingIsTrue()
    {
        var personC = CreateTestPerson("Charlie", "Wilson", "charlie@test.com");
        var personA = CreateTestPerson("Alice", "Anderson", "alice@test.com");
        var personB = CreateTestPerson("Bob", "Brown", "bob@test.com");

        _context.Set<Person<int>>().AddRange(personC, personA, personB);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(
            isSorted: true,
            orderBy: p => p.FirstName,
            ascending: true);

        var list = result.ToList();
        list[0].FirstName.Should().Be("Alice");
        list[1].FirstName.Should().Be("Bob");
        list[2].FirstName.Should().Be("Charlie");
    }

    [Fact]
    public async Task Should_SortDescending_When_AscendingIsFalse()
    {
        var personA = CreateTestPerson("Alice", "Anderson", "alice@test.com");
        var personB = CreateTestPerson("Bob", "Brown", "bob@test.com");
        var personC = CreateTestPerson("Charlie", "Wilson", "charlie@test.com");

        _context.Set<Person<int>>().AddRange(personA, personB, personC);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(
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
