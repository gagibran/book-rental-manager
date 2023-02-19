namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBooksByQueryParametersFromAuthorQueryHandlerTestsQuery
{
    private readonly GetBooksByQueryParametersFromAuthorQuery _getBooksByQueryParametersFromAuthorQuery;
    private readonly Author _author;
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, GetBookDto>> _bookToGetBookDtoMapperStub;
    private readonly Mock<IMapper<BookSortParameters, string>> _bookSortParametersMapperStub;
    private readonly GetBooksByQueryParametersFromAuthorQueryHandler _getBooksByQueryParametersFromAuthorQueryHandler;

    public GetBooksByQueryParametersFromAuthorQueryHandlerTestsQuery()
    {
        _author = TestFixtures.CreateDummyAuthor();
        _getBooksByQueryParametersFromAuthorQuery = new(
            _author.Id,
            It.IsAny<int>(),
            It.IsAny<int>(),
            string.Empty,
            It.IsAny<string>());
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _bookToGetBookDtoMapperStub = new();
        _bookSortParametersMapperStub = new();
        _getBooksByQueryParametersFromAuthorQueryHandler = new GetBooksByQueryParametersFromAuthorQueryHandler(
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
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersFromAuthorQueryHandler.HandleAsync(
            _getBooksByQueryParametersFromAuthorQuery,
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
            book.IsAvailable,
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
            .Returns(string.Empty);

        // Act:
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersFromAuthorQueryHandler.HandleAsync(
            _getBooksByQueryParametersFromAuthorQuery,
            It.IsAny<CancellationToken>());

        // Assert (maybe refactor this using FluentAssertions):
        GetBookDto actualGetBookDto = handlerResult.Value!.FirstOrDefault()!;
        Assert.Equal(getBookDto.Id, actualGetBookDto.Id);
        Assert.Equal(getBookDto.BookTitle, actualGetBookDto.BookTitle);
        Assert.Equal(getBookDto.Authors, actualGetBookDto.Authors);
        Assert.Equal(getBookDto.Edition, actualGetBookDto.Edition);
        Assert.Equal(getBookDto.Isbn, actualGetBookDto.Isbn);
        Assert.Equal(getBookDto.IsAvailable, actualGetBookDto.IsAvailable);
        Assert.Equal(getBookDto.RentedBy, actualGetBookDto.RentedBy);
    }
}
