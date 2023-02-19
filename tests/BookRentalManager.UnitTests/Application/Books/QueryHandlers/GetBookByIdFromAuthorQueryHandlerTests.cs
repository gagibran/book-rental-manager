namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBookByIdFromAuthorQueryHandlerTests
{
    private readonly Mock<IRepository<Author>> _authorRepositoryStub;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, GetBookDto>> _bookToGetBookDtoMapperStub;
    private readonly GetBookByIdFromAuthorQueryHandler _getBookByIdFromAuthorQueryHandler;
    private readonly Author _author;
    private readonly Book _book;
    private readonly GetBookDto _getBookDto;

    public GetBookByIdFromAuthorQueryHandlerTests()
    {
        _author = TestFixtures.CreateDummyAuthor();
        _book = TestFixtures.CreateDummyBook();
        _getBookDto = new(
            _book.Id,
            _book.BookTitle,
            new List<GetAuthorFromBookDto>(),
            _book.Edition,
            _book.Isbn,
            _book.IsAvailable,
            new GetCustomerThatRentedBookDto());
        _bookToGetBookDtoMapperStub = new();
        _bookRepositoryStub = new();
        _authorRepositoryStub = new();
        _getBookByIdFromAuthorQueryHandler = new(
            _authorRepositoryStub.Object,
            _bookRepositoryStub.Object,
            _bookToGetBookDtoMapperStub.Object);
        _bookToGetBookDtoMapperStub
            .Setup(bookToGetBookDtoMapper => bookToGetBookDtoMapper.Map(It.IsAny<Book>()))
            .Returns(_getBookDto);
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Author>>(),
                default))
            .ReturnsAsync(_author);
    }

    [Fact]
    public async Task HandleAsync_WithAuthorAndId_ReturnsBook()
    {
        // Arrange:
        _author.AddBook(_book);
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(_book);

        // Act:
        Result<GetBookDto> handlerResult = await _getBookByIdFromAuthorQueryHandler.HandleAsync(
            new GetBookByIdFromAuthorQuery(_author.Id, _book.Id),
            default);

        // Assert:
        Assert.Equal(_getBookDto.Id, handlerResult.Value!.Id);
    }

    [Fact]
    public async Task HandleAsync_WithAuthorAndNonexistingId_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"No book with the ID of '{_book.Id}' was found for this author.";
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync((Book)null!);

        // Act:
        Result<GetBookDto> handlerResult = await _getBookByIdFromAuthorQueryHandler.HandleAsync(
            new GetBookByIdFromAuthorQuery(_author.Id, _book.Id),
            default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"No author with the ID of '{_author.Id}' was found.";
        _authorRepositoryStub
            .Setup(authorRepository => authorRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Author>>(),
                default))
            .ReturnsAsync((Author)null!);

        // Act:
        Result<GetBookDto> handlerResult = await _getBookByIdFromAuthorQueryHandler.HandleAsync(
            new GetBookByIdFromAuthorQuery(_author.Id, _book.Id),
            default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }
}
