namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBooksByQueryParametersExcludingFromAuthorQueryHandlerTestsQuery
{
    private readonly GetBooksByQueryParametersExcludingFromAuthorQuery _getBooksByQueryParametersExcludingFromAuthorQuery;
    private readonly Author _author;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, GetBookDto>> _bookToGetBookDtoMapperStub;
    private readonly Mock<IMapper<BookSortParameters, Result<string>>> _bookSortParametersMapperStub;
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
        _bookToGetBookDtoMapperStub = new();
        _bookSortParametersMapperStub = new();
        _getBooksByQueryParametersExcludingFromAuthorQueryHandler = new GetBooksByQueryParametersExcludingFromAuthorQueryHandler(
            _authorRepositoryStub.Object,
            _bookRepositoryStub.Object,
            _bookToGetBookDtoMapperStub.Object,
            _bookSortParametersMapperStub.Object);
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
            new List<Book> { book },
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>());
        var getBookDto = new GetBookDto(
            Guid.NewGuid(),
            book.BookTitle,
            new List<GetAuthorFromBookDto>(),
            book.Edition,
            book.Isbn,
            book.RentedAt,
            book.DueDate,
            new GetCustomerThatRentedBookDto());
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
        _bookToGetBookDtoMapperStub
            .Setup(_bookToGetBookDtoMapper => _bookToGetBookDtoMapper.Map(It.IsAny<Book>()))
            .Returns(getBookDto);
        _bookSortParametersMapperStub
            .Setup(bookSortParametersMapper => bookSortParametersMapper.Map(It.IsAny<BookSortParameters>()))
            .Returns(Result.Success(string.Empty));

        // Act:
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersExcludingFromAuthorQueryHandler.HandleAsync(
            _getBooksByQueryParametersExcludingFromAuthorQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        GetBookDto actualGetBookDto = handlerResult.Value!.FirstOrDefault()!;
        Assert.Equal(getBookDto.Id, actualGetBookDto.Id);
        Assert.Equal(getBookDto.BookTitle, actualGetBookDto.BookTitle);
        Assert.Equal(getBookDto.Authors, actualGetBookDto.Authors);
        Assert.Equal(getBookDto.Edition, actualGetBookDto.Edition);
        Assert.Equal(getBookDto.Isbn, actualGetBookDto.Isbn);
        Assert.Equal(getBookDto.RentedAt, actualGetBookDto.RentedAt);
        Assert.Equal(getBookDto.DueDate, actualGetBookDto.DueDate);
        Assert.Equal(getBookDto.RentedBy, actualGetBookDto.RentedBy);
    }
}
