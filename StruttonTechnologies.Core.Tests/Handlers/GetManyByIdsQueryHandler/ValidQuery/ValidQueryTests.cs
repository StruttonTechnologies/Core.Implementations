using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Queries;
using StruttonTechnologies.Core.Coordinator.Crud.Handlers;
using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Tests.Handlers.GetManyByIdsQueryHandler.ValidQuery;

public class ValidQueryTests
{
    [Fact]
    public async Task Should_ReturnResults_When_QueryIsValid()
    {
        var mockRepository = new Mock<IReadRepository<PersonDto, int>>();
        var mockLogger = new Mock<ILogger<GetManyByIdsQueryHandler<PersonDto, int>>>();
        var handler = new GetManyByIdsQueryHandler<PersonDto, int>(mockRepository.Object, mockLogger.Object);

        var expectedDtos = new List<PersonDto>
        {
            new PersonDto { Id = "1", FirstName = "John", LastName = "Doe", Email = "john@test.com" },
            new PersonDto { Id = "2", FirstName = "Jane", LastName = "Smith", Email = "jane@test.com" }
        };

        var ids = new[] { 1, 2 };
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
            .ReturnsAsync(expectedDtos);

        var result = await handler.Handle(query, default);

        result.Should().HaveCount(2);
        result.Should().Contain(dto => dto.FirstName == "John");
        result.Should().Contain(dto => dto.FirstName == "Jane");
    }

    [Fact]
    public async Task Should_PassIdsToRepository_When_Handling()
    {
        var mockRepository = new Mock<IReadRepository<PersonDto, int>>();
        var mockLogger = new Mock<ILogger<GetManyByIdsQueryHandler<PersonDto, int>>>();
        var handler = new GetManyByIdsQueryHandler<PersonDto, int>(mockRepository.Object, mockLogger.Object);

        var ids = new[] { 1, 2, 3 };
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

        await handler.Handle(query, default);

        mockRepository.Verify(r => r.GetManyByIdsAsync(
            ids,
            It.IsAny<bool>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<PersonDto, int>>>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<PersonDto, object>>[]>()), Times.Once);
    }
}
