using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.CrudRepository.GetAllAsync.Pagination;

public class PaginationBehaviorTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly CrudRepository<Person<int>, int> _repository;

    public PaginationBehaviorTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new CrudRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_ReturnFirstPage_When_PageNumberIsOne()
    {
        for (int i = 1; i <= 5; i++)
        {
            _context.Set<Person<int>>().Add(CreateTestPerson($"Person{i}", "Doe", $"person{i}@test.com"));
        }
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync<string>(
            isPaginated: true,
            pageNumber: 1,
            pageSize: 2);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Should_ReturnSecondPage_When_PageNumberIsTwo()
    {
        for (int i = 1; i <= 5; i++)
        {
            _context.Set<Person<int>>().Add(CreateTestPerson($"Person{i}", "Doe", $"person{i}@test.com"));
        }
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(
            isSorted: true,
            orderBy: p => p.FirstName,
            ascending: true,
            isPaginated: true,
            pageNumber: 2,
            pageSize: 2);

        result.Should().HaveCount(2);
        var list = result.ToList();
        list[0].FirstName.Should().Be("Person3");
        list[1].FirstName.Should().Be("Person4");
    }

    [Fact]
    public async Task Should_ReturnRemainingItems_When_LastPageIsPartial()
    {
        for (int i = 1; i <= 5; i++)
        {
            _context.Set<Person<int>>().Add(CreateTestPerson($"Person{i}", "Doe", $"person{i}@test.com"));
        }
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync<string>(
            isPaginated: true,
            pageNumber: 3,
            pageSize: 2);

        result.Should().HaveCount(1);
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
