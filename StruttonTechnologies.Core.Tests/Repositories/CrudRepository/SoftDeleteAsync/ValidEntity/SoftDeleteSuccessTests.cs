using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.CrudRepository.SoftDeleteAsync.ValidEntity;

public class SoftDeleteSuccessTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly CrudRepository<Person<int>, int> _repository;

    public SoftDeleteSuccessTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new CrudRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_SetIsDeletedToTrue_When_EntityExists()
    {
        var person = CreateTestPerson("John", "Doe", "john@test.com");
        _context.Set<Person<int>>().Add(person);
        await _context.SaveChangesAsync();
        var entityId = person.Id;

        var result = await _repository.SoftDeleteAsync(entityId);

        result.Should().NotBeNull();
        result!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnDeletedEntity_When_SoftDeleteSucceeds()
    {
        var person = CreateTestPerson("Jane", "Smith", "jane@test.com");
        _context.Set<Person<int>>().Add(person);
        await _context.SaveChangesAsync();
        var entityId = person.Id;

        var result = await _repository.SoftDeleteAsync(entityId);

        result.Should().NotBeNull();
        result!.Email.Should().Be("jane@test.com");
        result.FirstName.Should().Be("Jane");
    }

    [Fact]
    public async Task Should_PersistChanges_When_SoftDeleteCompletes()
    {
        var person = CreateTestPerson("Bob", "Johnson", "bob@test.com");
        _context.Set<Person<int>>().Add(person);
        await _context.SaveChangesAsync();
        var entityId = person.Id;

        await _repository.SoftDeleteAsync(entityId);

        _context.ChangeTracker.Clear();
        var retrieved = await _context.Set<Person<int>>().FindAsync(entityId);
        retrieved.Should().NotBeNull();
        retrieved!.IsDeleted.Should().BeTrue();
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
