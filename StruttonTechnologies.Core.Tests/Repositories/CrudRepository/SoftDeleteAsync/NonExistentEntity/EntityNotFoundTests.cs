using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.CrudRepository.SoftDeleteAsync.NonExistentEntity;

public class EntityNotFoundTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly CrudRepository<Person<int>, int> _repository;

    public EntityNotFoundTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new CrudRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_ReturnNull_When_EntityDoesNotExist()
    {
        var result = await _repository.SoftDeleteAsync(999);

        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
