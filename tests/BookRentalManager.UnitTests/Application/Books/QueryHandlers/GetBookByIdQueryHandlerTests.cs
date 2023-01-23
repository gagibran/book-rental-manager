namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IRepository<BookAuthor>> _bookAuthorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, GetBookDto>> _getBookDtoMapperStub;
    private readonly GetBookByIdQueryHandler _getBookByIdQueryHandler;
    private readonly BookAuthor _bookAuthor;
    private readonly Book _book;
    private readonly GetBookDto _getBookDto;

    public GetBookByIdQueryHandlerTests()
    {
        _bookAuthor = TestFixtures.CreateDummyBookAuthor();
        _book = TestFixtures.CreateDummyBook();
        _getBookDto = new(
            _book.Id,
            _book.BookTitle,
            new List<GetBookBookAuthorDto>(),
            _book.Edition,
            _book.Isbn,
            _book.IsAvailable,
            new GetRentedByDto());
        _getBookDtoMapperStub = new();
        _bookRepositoryStub = new();
        _bookAuthorRepositoryStub = new();
        _getBookByIdQueryHandler = new(
            _bookAuthorRepositoryStub.Object,
            _bookRepositoryStub.Object,
            _getBookDtoMapperStub.Object);
        _getBookDtoMapperStub
            .Setup(getBookDtoMapper => getBookDtoMapper.Map(It.IsAny<Book>()))
            .Returns(_getBookDto);
        _bookAuthorRepositoryStub
            .Setup(bookAuthorRepository => bookAuthorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<BookAuthor>>(),
                default))
            .ReturnsAsync(_bookAuthor);
    }

    [Fact]
    public async Task HandleAsync_WithBookAuthorAndId_ReturnsBook()
    {
        // Arrange:
        _bookAuthor.AddBook(_book);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(_book);

        // Act:
        Result<GetBookDto> handlerResult = await _getBookByIdQueryHandler.HandleAsync(
            new GetBookByIdQuery(_bookAuthor.Id, _book.Id),
            default);

        // Assert:
        Assert.Equal(_getBookDto.Id, handlerResult.Value.Id);
    }

    [Fact]
    public async Task HandleAsync_WithBookAuthorAndNonexistingId_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"No book with the ID of '{_book.Id}' was found for this book author.";
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync((Book)null);

        // Act:
        Result<GetBookDto> handlerResult = await _getBookByIdQueryHandler.HandleAsync(
            new GetBookByIdQuery(_bookAuthor.Id, _book.Id),
            default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingBookAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"No book author with the ID of '{_bookAuthor.Id}' was found.";
        _bookAuthorRepositoryStub
            .Setup(bookAuthorRepository => bookAuthorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<BookAuthor>>(),
                default))
            .ReturnsAsync((BookAuthor)null);

        // Act:
        Result<GetBookDto> handlerResult = await _getBookByIdQueryHandler.HandleAsync(
            new GetBookByIdQuery(_bookAuthor.Id, _book.Id),
            default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }
}
