namespace BookRentalManager.UnitTests.Application.Authors.QueryHandlers;

public sealed class GetAuthorsByQueryParametersQueryHandlerTests
{
    private readonly GetAuthorsByQueryParametersQuery _getAuthorsByQueryParametersQuery;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IMapper<Author, GetAuthorDto>> _authorToGetAuthorDtoMapperStub;
    private readonly Mock<IMapper<AuthorSortParameters, Result<string>>> _authorSortParametersMapperStub;
    private readonly GetAuthorsByQueryParametersQueryHandler _getAuthorsByQueryParametersQueryHandler;

    public GetAuthorsByQueryParametersQueryHandlerTests()
    {
        _getAuthorsByQueryParametersQuery = new(
            It.IsAny<int>(),
            It.IsAny<int>(),
            string.Empty,
            It.IsAny<string>());
        _authorToGetAuthorDtoMapperStub = new();
        _authorSortParametersMapperStub = new();
        _authorRepositoryStub = new();
        _getAuthorsByQueryParametersQueryHandler = new(
            _authorRepositoryStub.Object,
            _authorToGetAuthorDtoMapperStub.Object,
            _authorSortParametersMapperStub.Object);
        _authorSortParametersMapperStub
            .Setup(authorSortParametersMapper => authorSortParametersMapper.Map(It.IsAny<AuthorSortParameters>()))
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
                new List<Author>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()));
        _authorToGetAuthorDtoMapperStub
            .Setup(authorToGetAuthorDtoMapper => authorToGetAuthorDtoMapper.Map(It.IsAny<Author>()))
            .Returns(new GetAuthorDto(
                It.IsAny<Guid>(),
                FullName.Create("John", "Doe").Value!,
                It.IsAny<IReadOnlyList<GetBookFromAuthorDto>>()));

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
        var expectedGetAuthorDto = new GetAuthorDto(Guid.NewGuid(), author.FullName, new List<GetBookFromAuthorDto>());
        var paginatedAuthors = new PaginatedList<Author>(
            new List<Author> { author },
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
        _authorToGetAuthorDtoMapperStub
            .Setup(authorToGetAuthorDtoMapper => authorToGetAuthorDtoMapper.Map(It.IsAny<Author>()))
            .Returns(expectedGetAuthorDto);

        // Act:
        Result<PaginatedList<GetAuthorDto>> handlerResult = await _getAuthorsByQueryParametersQueryHandler.HandleAsync(
            _getAuthorsByQueryParametersQuery,
            It.IsAny<CancellationToken>());

        // Assert (maybe refactor this using FluentAssertions):
        GetAuthorDto actualGetAuthorDto = handlerResult.Value!.FirstOrDefault()!;
        Assert.Equal(expectedGetAuthorDto.Id, actualGetAuthorDto.Id);
        Assert.Equal(expectedGetAuthorDto.FullName, actualGetAuthorDto.FullName);
        Assert.Equal(expectedGetAuthorDto.Books, actualGetAuthorDto.Books);
    }
}
