using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Repositories.Person;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.PersonRepository.GetByEmail;

public class NonExistentEmailTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly PersonRepository<Person<int>, int> _repository;

    public NonExistentEmailTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new PersonRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_ReturnNull_When_EmailDoesNotExist()
    {
        var result = await _repository.GetByEmailAsync("nonexistent@test.com");

        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
