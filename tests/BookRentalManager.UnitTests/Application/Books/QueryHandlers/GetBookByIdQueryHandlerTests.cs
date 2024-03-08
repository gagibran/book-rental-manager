namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBookByIdQueryHandlerTests
{
    private readonly Book _book;
    private readonly GetBookDto _getBookDto;
    private readonly GetBookByIdQuery _getBookByIdQuery;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly GetBookByIdQueryHandler _getBookByIdQueryHandler;

    public GetBookByIdQueryHandlerTests()
    {
        _book = TestFixtures.CreateDummyBook();
        _getBookDto = new(
            _book.Id,
            _book.BookTitle,
            [],
            _book.Edition.EditionNumber,
            _book.Isbn.ToString(),
            _book.RentedAt,
            _book.DueDate,
            null);
        _getBookByIdQuery = new GetBookByIdQuery(_book.Id);
        _bookRepositoryStub = new();
        _getBookByIdQueryHandler = new(_bookRepositoryStub.Object);
    }

    [Fact]
    public async Task HandleAsync_WithNonexistingBook_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = $"No book with the ID of '{_book.Id}' was found.";
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<BookByIdWithAuthorsAndCustomersSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book)null!);

        // Act:
        Result<GetBookDto> handlerResult = await _getBookByIdQueryHandler.HandleAsync(
            _getBookByIdQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(expectedErrorMessage, handlerResult.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithExistingBook_ReturnsBook()
    {
        // Arrange:
        _bookRepositoryStub
            .Setup(bookRepository => bookRepository.GetFirstOrDefaultBySpecificationAsync(
                It.IsAny<Specification<Book>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_book);

        // Act:
        Result<GetBookDto> handlerResult = await _getBookByIdQueryHandler.HandleAsync(
            _getBookByIdQuery,
            It.IsAny<CancellationToken>());

        // Assert:
        Assert.Equal(_getBookDto.Id, handlerResult.Value!.Id);
    }
}
