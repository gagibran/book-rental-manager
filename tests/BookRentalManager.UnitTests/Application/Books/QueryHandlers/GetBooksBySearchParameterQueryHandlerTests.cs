namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBooksBySearchParameterQueryHandlerTests
{
    private readonly Book _book;
    private readonly List<Book> _books;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, GetBookDto>> _getBookDtoMapperStub;
    private readonly GetBookDto _getBookDto;
    private readonly GetBooksBySearchParameterQueryHandler _getBooksBySearchParameterQueryHandler;

    public GetBooksBySearchParameterQueryHandlerTests()
    {
        _bookRepositoryStub = new();
        _getBookDtoMapperStub = new();
        _book = TestFixtures.CreateDummyBook();
        _books = new List<Book> { _book };
        _getBookDto = new(
            Guid.NewGuid(),
            _book.BookTitle,
            new List<GetBookBookAuthorDto>(),
            _book.Edition,
            _book.Isbn,
            _book.IsAvailable,
            new GetRentedByDto());
        _getBooksBySearchParameterQueryHandler = new GetBooksBySearchParameterQueryHandler(
            _bookRepositoryStub.Object,
            _getBookDtoMapperStub.Object);
        _getBookDtoMapperStub
            .Setup(_getBookDtoMapper => _getBookDtoMapper.Map(It.IsAny<Book>()))
            .Returns(_getBookDto);
    }

    [Fact]
    public async Task HandleAsync_WithoutAnyBooksWithSearchParameter_ReturnsEmptyList()
    {
        // Assert:
        var getBooksBySearchParameterQuery = new GetBooksBySearchParameterQuery(
            TestFixtures.PageIndex,
            TestFixtures.TotalItemsPerPage,
            "Name");
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(new List<Book>());

        // Act:
        Result<IReadOnlyList<GetBookDto>> handlerResult = await _getBooksBySearchParameterQueryHandler.HandleAsync(
            getBooksBySearchParameterQuery,
            default);

        // Assert:
        Assert.Empty(handlerResult.Value);
    }

    [Fact]
    public async Task HandleAsync_WithAtLeastOneBookWithSearchParameter_ReturnsListWithMatchingBook()
    {
        // Assert:
        var getBooksBySearchParameterQuery = new GetBooksBySearchParameterQuery(
            TestFixtures.PageIndex,
            TestFixtures.TotalItemsPerPage,
            _book.BookTitle);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(_books);

        // Act:
        Result<IReadOnlyList<GetBookDto>> handlerResult = await _getBooksBySearchParameterQueryHandler.HandleAsync(
            getBooksBySearchParameterQuery,
            default);
        GetBookDto actualBookDto = handlerResult.Value.FirstOrDefault();

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(_getBookDto.Id, actualBookDto.Id);
        Assert.Equal(_getBookDto.BookTitle, actualBookDto.BookTitle);
        Assert.Equal(_getBookDto.BookAuthors, actualBookDto.BookAuthors);
        Assert.Equal(_getBookDto.Edition, actualBookDto.Edition);
        Assert.Equal(_getBookDto.Isbn, actualBookDto.Isbn);
        Assert.Equal(_getBookDto.IsAvailable, actualBookDto.IsAvailable);
        Assert.Equal(_getBookDto.RentedBy.FullName, actualBookDto.RentedBy.FullName);
        Assert.Equal(_getBookDto.RentedBy.Email, actualBookDto.RentedBy.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task HandleAsync_WithEmptySearchParameter_ReturnsListWithAllBooks(string searchParam)
    {
        // Arrange:
        var getBooksBySearchParameterQuery = new GetBooksBySearchParameterQuery(
            TestFixtures.PageIndex,
            TestFixtures.TotalItemsPerPage,
            searchParam);
        var newBook = new Book(
            "Clean Code: A Handbook of Agile Software Craftsmanship",
            Edition.Create(1).Value,
            Isbn.Create("978-0132350884").Value);
        _books.Add(newBook);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetAllBySpecificationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(_books);

        // Act:
        Result<IReadOnlyList<GetBookDto>> handlerResult = await _getBooksBySearchParameterQueryHandler.HandleAsync(
            getBooksBySearchParameterQuery,
            default);

        // Assert:
        Assert.Equal(2, handlerResult.Value.Count);
    }
}
