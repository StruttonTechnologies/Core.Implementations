using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.CrudRepository.GetManyByIdsAsync.ValidIds;

public class MultipleIdsTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly CrudRepository<Person<int>, int> _repository;

    public MultipleIdsTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new CrudRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_ReturnMatchingEntities_When_IdsExist()
    {
        var person1 = CreateTestPerson("John", "Doe", "john@test.com");
        var person2 = CreateTestPerson("Jane", "Smith", "jane@test.com");
        var person3 = CreateTestPerson("Bob", "Johnson", "bob@test.com");

        _context.Set<Person<int>>().AddRange(person1, person2, person3);
        await _context.SaveChangesAsync();

        var ids = new[] { person1.Id, person3.Id };
        var result = await _repository.GetManyByIdsAsync(ids);

        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Email == "john@test.com");
        result.Should().Contain(p => p.Email == "bob@test.com");
        result.Should().NotContain(p => p.Email == "jane@test.com");
    }

    [Fact]
    public async Task Should_ReturnEmpty_When_NoIdsMatch()
    {
        var person1 = CreateTestPerson("John", "Doe", "john@test.com");
        _context.Set<Person<int>>().Add(person1);
        await _context.SaveChangesAsync();

        var ids = new[] { 999, 998 };
        var result = await _repository.GetManyByIdsAsync(ids);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_ReturnAllMatching_When_SomeIdsMatch()
    {
        var person1 = CreateTestPerson("John", "Doe", "john@test.com");
        var person2 = CreateTestPerson("Jane", "Smith", "jane@test.com");

        _context.Set<Person<int>>().AddRange(person1, person2);
        await _context.SaveChangesAsync();

        var ids = new[] { person1.Id, 999 };
        var result = await _repository.GetManyByIdsAsync(ids);

        result.Should().HaveCount(1);
        result.Should().Contain(p => p.Email == "john@test.com");
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
