using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Queries;
using StruttonTechnologies.Core.Coordinator.Crud.Handlers;
using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Tests.Handlers.GetManyByIdsQueryHandler.EmptyResults;

public class EmptyResultsTests
{
    [Fact]
    public async Task Should_ReturnEmptyCollection_When_NoEntitiesFound()
    {
        var mockRepository = new Mock<IReadRepository<PersonDto, int>>();
        var mockLogger = new Mock<ILogger<GetManyByIdsQueryHandler<PersonDto, int>>>();
        var handler = new GetManyByIdsQueryHandler<PersonDto, int>(mockRepository.Object, mockLogger.Object);

        var ids = new[] { 999, 998 };
        var query = new GetManyByIdsQuery<PersonDto, int>(ids);

        mockRepository
            .Setup(r => r.GetManyByIdsAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<bool>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<PersonDto, int>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<PersonDto, object>>[]>()))
            .ReturnsAsync(new List<PersonDto>());

        var result = await handler.Handle(query, default);

        result.Should().BeEmpty();
    }
}
