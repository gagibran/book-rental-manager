namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, GetBookDto>> _getBookDtoMapperStub;
    private readonly GetBookByIdQueryHandler _getBookByIdQueryHandler;
    private readonly Book _book;
    private readonly GetBookDto _getBookDto;

    public GetBookByIdQueryHandlerTests()
    {
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
        _getBookByIdQueryHandler = new(
            _bookRepositoryStub.Object,
            _getBookDtoMapperStub.Object);
        _getBookDtoMapperStub
            .Setup(getBookDtoMapper => getBookDtoMapper.Map(It.IsAny<Book>()))
            .Returns(_getBookDto);
    }

    [Fact]
    public async Task HandleAsync_WithBookWithId_ReturnsBook()
    {
        // Assert:
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync(_book);

        // Act:
        Result<GetBookDto> handlerResult = await _getBookByIdQueryHandler.HandleAsync(
            new GetBookByIdQuery(_book.Id),
            default);

        // Assert:
        Assert.Equal(_getBookDto.Id, handlerResult.Value.Id);
    }

    [Fact]
    public async Task HandleAsync_WithBookWithNonexistingId_ReturnsErrorMessage()
    {
        // Assert:
        var expectedErrorMessage = $"No book with the ID of '{_book.Id} was found.";
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Book>>(),
                default))
            .ReturnsAsync((Book)null);

        // Act:
        Result<GetBookDto> handlerResult = await _getBookByIdQueryHandler.HandleAsync(
            new GetBookByIdQuery(_book.Id),
            default);

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }
}
