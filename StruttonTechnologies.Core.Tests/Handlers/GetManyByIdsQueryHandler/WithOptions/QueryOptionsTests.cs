using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Queries;
using StruttonTechnologies.Core.Coordinator.Crud.Handlers;
using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Tests.Handlers.GetManyByIdsQueryHandler.WithOptions;

public class QueryOptionsTests
{
    [Fact]
    public async Task Should_PassSortingParameters_When_SortingIsEnabled()
    {
        var mockRepository = new Mock<IReadRepository<PersonDto, int>>();
        var mockLogger = new Mock<ILogger<GetManyByIdsQueryHandler<PersonDto, int>>>();
        var handler = new GetManyByIdsQueryHandler<PersonDto, int>(mockRepository.Object, mockLogger.Object);

        var ids = new[] { 1, 2 };
        var query = new GetManyByIdsQuery<PersonDto, int>(
            Ids: ids,
            IsSorted: true,
            Ascending: false);

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

        await handler.Handle(query, default);

        mockRepository.Verify(r => r.GetManyByIdsAsync(
            ids,
            true,
            It.IsAny<System.Linq.Expressions.Expression<Func<PersonDto, int>>>(),
            false,
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<PersonDto, object>>[]>()), Times.Once);
    }

    [Fact]
    public async Task Should_PassPaginationParameters_When_PaginationIsEnabled()
    {
        var mockRepository = new Mock<IReadRepository<PersonDto, int>>();
        var mockLogger = new Mock<ILogger<GetManyByIdsQueryHandler<PersonDto, int>>>();
        var handler = new GetManyByIdsQueryHandler<PersonDto, int>(mockRepository.Object, mockLogger.Object);

        var ids = new[] { 1, 2, 3 };
        var query = new GetManyByIdsQuery<PersonDto, int>(
            Ids: ids,
            IsPaginated: true,
            PageNumber: 2,
            PageSize: 10);

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

        await handler.Handle(query, default);

        mockRepository.Verify(r => r.GetManyByIdsAsync(
            ids,
            It.IsAny<bool>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<PersonDto, int>>>(),
            It.IsAny<bool>(),
            true,
            2,
            10,
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<PersonDto, object>>[]>()), Times.Once);
    }
}
