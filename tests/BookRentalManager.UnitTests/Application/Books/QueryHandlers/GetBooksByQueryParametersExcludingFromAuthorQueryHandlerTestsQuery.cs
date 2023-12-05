namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBooksByQueryParametersExcludingFromAuthorQueryHandlerTestsQuery
{
    private readonly GetBooksByQueryParametersExcludingFromAuthorQuery _getBooksByQueryParametersExcludingFromAuthorQuery;
    private readonly Author _author;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<ISortParametersMapper> _sortParametersMapperStub;
    private readonly GetBooksByQueryParametersExcludingFromAuthorQueryHandler _getBooksByQueryParametersExcludingFromAuthorQueryHandler;

    public GetBooksByQueryParametersExcludingFromAuthorQueryHandlerTestsQuery()
    {
        _author = TestFixtures.CreateDummyAuthor();
        _getBooksByQueryParametersExcludingFromAuthorQuery = new(
            _author.Id,
            It.IsAny<int>(),
            It.IsAny<int>(),
            string.Empty,
            It.IsAny<string>());
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _sortParametersMapperStub = new();
        _getBooksByQueryParametersExcludingFromAuthorQueryHandler = new GetBooksByQueryParametersExcludingFromAuthorQueryHandler(
            _authorRepositoryStub.Object,
            _bookRepositoryStub.Object,
            _sortParametersMapperStub.Object);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"No author with the ID of '{_author.Id}' was found.";
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Author>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author)null!);

        // Act:
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersExcludingFromAuthorQueryHandler.HandleAsync(
            _getBooksByQueryParametersExcludingFromAuthorQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithExistingAuthor_ReturnsExpectedGetBookDtos()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var paginatedBooks = new PaginatedList<Book>(
            [book],
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>());
        var expectedGetBookDto = new GetBookDto(
            book.Id,
            book.BookTitle,
            new List<GetAuthorFromBookDto>(),
            book.Edition,
            book.Isbn,
            book.RentedAt,
            book.DueDate,
            null);
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Author>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_author);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Book>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedBooks);
        _sortParametersMapperStub
            .Setup(sortParametersMapper => sortParametersMapper.MapBookSortParameters(It.IsAny<string>()))
            .Returns(Result.Success(string.Empty));

        // Act:
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersExcludingFromAuthorQueryHandler.HandleAsync(
            _getBooksByQueryParametersExcludingFromAuthorQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        GetBookDto actualGetBookDto = handlerResult.Value!.FirstOrDefault()!;
        Assert.Equal(expectedGetBookDto.Id, actualGetBookDto.Id);
        Assert.Equal(expectedGetBookDto.BookTitle, actualGetBookDto.BookTitle);
        Assert.Equal(expectedGetBookDto.Authors, actualGetBookDto.Authors);
        Assert.Equal(expectedGetBookDto.Edition, actualGetBookDto.Edition);
        Assert.Equal(expectedGetBookDto.Isbn, actualGetBookDto.Isbn);
        Assert.Equal(expectedGetBookDto.RentedAt, actualGetBookDto.RentedAt);
        Assert.Equal(expectedGetBookDto.DueDate, actualGetBookDto.DueDate);
        Assert.Equal(expectedGetBookDto.RentedBy, actualGetBookDto.RentedBy);
    }
}
