namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBooksByQueryParametersFromAuthorQueryHandlerTestsQuery
{
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Author _author;
    private readonly Book _book;
    private readonly PaginatedList<Book> _paginatedBooks;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, GetBookDto>> _bookToGetBookDtoMapperStub;
    private readonly GetBookDto _getBookDto;
    private readonly GetBooksByQueryParametersFromAuthorQueryHandler _getBooksByQueryParametersFromAuthorQueryHandler;

    public GetBooksByQueryParametersFromAuthorQueryHandlerTestsQuery()
    {
        _author = TestFixtures.CreateDummyAuthor();
        _authorRepositoryStub = new();
        _bookRepositoryStub = new();
        _bookToGetBookDtoMapperStub = new();
        _book = TestFixtures.CreateDummyBook();
        _paginatedBooks = new PaginatedList<Book>(new List<Book> { _book }, 1, 1, 1, 1);
        _getBookDto = new(
            Guid.NewGuid(),
            _book.BookTitle,
            new List<GetAuthorFromBookDto>(),
            _book.Edition,
            _book.Isbn,
            _book.IsAvailable,
            new GetCustomerThatRentedBookDto());
        _getBooksByQueryParametersFromAuthorQueryHandler = new GetBooksByQueryParametersFromAuthorQueryHandler(
            _authorRepositoryStub.Object,
            _bookRepositoryStub.Object,
            _bookToGetBookDtoMapperStub.Object);
        _bookToGetBookDtoMapperStub
            .Setup(_bookToGetBookDtoMapper => _bookToGetBookDtoMapper.Map(It.IsAny<Book>()))
            .Returns(_getBookDto);
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Author>>(),
                default))
            .ReturnsAsync(_author);
    }

    [Fact]
    public async Task HandleAsync_WithoutAnyBooksWithQueryParameter_ReturnsEmptyList()
    {
        // Arrange:
        var getBooksByQueryParametersFromAuthorQuery = new GetBooksByQueryParametersFromAuthorQuery(
            _author.Id,
            TestFixtures.PageIndex,
            TestFixtures.PageSize,
            "Name");
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(new PaginatedList<Book>(new List<Book>(), 1, 1, 1, 1));

        // Act:
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersFromAuthorQueryHandler.HandleAsync(
            getBooksByQueryParametersFromAuthorQuery,
            default);

        // Assert:
        Assert.Empty(handlerResult.Value);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneBookWithQueryParameter_ReturnsListWithMatchingBook()
    {
        // Arrange:
        var getBooksByQueryParametersFromAuthorQuery = new GetBooksByQueryParametersFromAuthorQuery(
            _author.Id,
            TestFixtures.PageIndex,
            TestFixtures.PageSize,
            _book.BookTitle);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(_paginatedBooks);

        // Act:
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersFromAuthorQueryHandler.HandleAsync(
            getBooksByQueryParametersFromAuthorQuery,
            default);
        GetBookDto actualBookDto = handlerResult.Value.FirstOrDefault();

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(_getBookDto.Id, actualBookDto.Id);
        Assert.Equal(_getBookDto.BookTitle, actualBookDto.BookTitle);
        Assert.Equal(_getBookDto.Authors, actualBookDto.Authors);
        Assert.Equal(_getBookDto.Edition, actualBookDto.Edition);
        Assert.Equal(_getBookDto.Isbn, actualBookDto.Isbn);
        Assert.Equal(_getBookDto.IsAvailable, actualBookDto.IsAvailable);
        Assert.Equal(_getBookDto.RentedBy.FullName, actualBookDto.RentedBy.FullName);
        Assert.Equal(_getBookDto.RentedBy.Email, actualBookDto.RentedBy.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task HandleAsync_WithEmptyQueryParameter_ReturnsListWithAllBooks(string searchParameter)
    {
        // Arrange:
        var getBooksByQueryParametersFromAuthorQuery = new GetBooksByQueryParametersFromAuthorQuery(
            _author.Id,
            TestFixtures.PageIndex,
            TestFixtures.PageSize,
            searchParameter);
        var newBook = new Book(
            "Clean Code: A Handbook of Agile Software Craftsmanship",
            Edition.Create(1).Value,
            Isbn.Create("978-0132350884").Value);
        _paginatedBooks.Add(newBook);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(_paginatedBooks);

        // Act:
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersFromAuthorQueryHandler.HandleAsync(
            getBooksByQueryParametersFromAuthorQuery,
            default);

        // Assert:
        Assert.Equal(2, handlerResult.Value.Count);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnErrorMessage()
    {
        // Arrange:
        var getBooksByQueryParametersFromAuthorQuery = new GetBooksByQueryParametersFromAuthorQuery(
            _author.Id,
            TestFixtures.PageIndex,
            TestFixtures.PageSize,
            "Test");
        var expectedErrorMessage = $"No author with the ID of '{_author.Id}' was found.";
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Author>>(),
                default))
            .ReturnsAsync((Author)null);

        // Act:
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersFromAuthorQueryHandler.HandleAsync(
            getBooksByQueryParametersFromAuthorQuery,
            default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }
}
