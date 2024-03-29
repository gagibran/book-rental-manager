namespace BookRentalManager.UnitTests.Application.Authors.QueryHandlers;

public sealed class GetAuthorByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IMapper<Author, GetAuthorDto>> _authorToGetAuthorDtoMapperStub;
    private readonly GetAuthorByIdQueryHandler _getAuthorByIdQueryHandler;
    private readonly Author _author;
    private readonly GetAuthorDto _getAuthorDto;

    public GetAuthorByIdQueryHandlerTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
        _getAuthorDto = new(
            _author.Id,
            _author.FullName,
            new List<GetBookFromAuthorDto>());
        _authorToGetAuthorDtoMapperStub = new();
        _authorRepositoryStub = new();
        _getAuthorByIdQueryHandler = new(
            _authorRepositoryStub.Object,
            _authorToGetAuthorDtoMapperStub.Object);
        _authorToGetAuthorDtoMapperStub
            .Setup(authorToGetAuthorDtoMapper => authorToGetAuthorDtoMapper.Map(It.IsAny<Author>()))
            .Returns(_getAuthorDto);
    }

    [Fact]
    public async Task HandleAsync_WithAuthorWithId_ReturnsAuthor()
    {
        // Arrange:
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_author);

        // Act:
        Result<GetAuthorDto> handlerResult = await _getAuthorByIdQueryHandler.HandleAsync(
            new GetAuthorByIdQuery(_author.Id),
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(_getAuthorDto.Id, handlerResult.Value!.Id);
    }

    [Fact]
    public async Task HandleAsync_WithAuthorWithNonexistingId_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"No author with the ID of '{_author.Id}' was found.";
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<AuthorByIdWithBooksSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author)null!);

        // Act:
        Result<GetAuthorDto> handlerResult = await _getAuthorByIdQueryHandler.HandleAsync(
            new GetAuthorByIdQuery(_author.Id),
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }
}
