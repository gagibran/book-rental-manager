namespace BookRentalManager.UnitTests.Application.Authors.QueryHandlers;

public sealed class GetAuthorsByQueryParametersQueryHandlerTests
{
    private readonly GetAuthorsByQueryParametersQuery _getAuthorsByQueryParametersQuery;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<ISortParametersMapper> _sortParametersMapperStub;
    private readonly GetAuthorsByQueryParametersQueryHandler _getAuthorsByQueryParametersQueryHandler;

    public GetAuthorsByQueryParametersQueryHandlerTests()
    {
        _getAuthorsByQueryParametersQuery = new(
            It.IsAny<int>(),
            It.IsAny<int>(),
            string.Empty,
            It.IsAny<string>());
        _sortParametersMapperStub = new();
        _authorRepositoryStub = new();
        _getAuthorsByQueryParametersQueryHandler = new(_authorRepositoryStub.Object, _sortParametersMapperStub.Object);
        _sortParametersMapperStub
            .Setup(sortParametersMapper => sortParametersMapper.MapAuthorSortParameters(It.IsAny<string>()))
            .Returns(Result.Success(string.Empty));
    }

    [Fact]
    public async Task HandleAsync_WithoutAuthors_ReturnsEmptyList()
    {
        // Arrange:
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Author>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Author>(
                [],
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()));

        // Act:
        Result<PaginatedList<GetAuthorDto>> handlerResult = await _getAuthorsByQueryParametersQueryHandler.HandleAsync(
            _getAuthorsByQueryParametersQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Empty(handlerResult.Value!);
    }

    [Fact]
    public async Task HandleAsync_WithAuthors_ReturnsExpectedGetAuthorDtos()
    {
        // Arrange:
        Author author = TestFixtures.CreateDummyAuthor();
        var expectedGetAuthorDto = new GetAuthorDto(author.Id, author.FullName.ToString(), []);
        var paginatedAuthors = new PaginatedList<Author>(
            [author],
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>());
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Author>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedAuthors);

        // Act:
        Result<PaginatedList<GetAuthorDto>> handlerResult = await _getAuthorsByQueryParametersQueryHandler.HandleAsync(
            _getAuthorsByQueryParametersQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        GetAuthorDto actualGetAuthorDto = handlerResult.Value!.FirstOrDefault()!;
        Assert.Equal(expectedGetAuthorDto.Id, actualGetAuthorDto.Id);
        Assert.Equal(expectedGetAuthorDto.FullName, actualGetAuthorDto.FullName);
        Assert.Equal(expectedGetAuthorDto.Books, actualGetAuthorDto.Books);
    }
}
