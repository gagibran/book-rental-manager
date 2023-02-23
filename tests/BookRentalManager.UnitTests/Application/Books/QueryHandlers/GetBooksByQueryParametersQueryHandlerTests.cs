namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBooksByQueryParametersQueryHandlerTests
{
    private readonly GetBooksByQueryParametersQuery _getBooksByQueryParametersQuery;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<IMapper<Book, GetBookDto>> _bookToGetBookDtoMapperStub;
    private readonly Mock<IMapper<BookSortParameters, Result<string>>> _bookSortParametersMapperStub;
    private readonly GetBooksByQueryParametersQueryHandler _getBooksByQueryParametersQueryHandler;

    public GetBooksByQueryParametersQueryHandlerTests()
    {
        _getBooksByQueryParametersQuery = new(
            It.IsAny<int>(),
            It.IsAny<int>(),
            string.Empty,
            It.IsAny<string>());
        _bookRepositoryStub = new();
        _bookToGetBookDtoMapperStub = new();
        _bookSortParametersMapperStub = new();
        _getBooksByQueryParametersQueryHandler = new GetBooksByQueryParametersQueryHandler(
            _bookRepositoryStub.Object,
            _bookToGetBookDtoMapperStub.Object,
            _bookSortParametersMapperStub.Object);
    }

    [Fact]
    public async Task HandleAsync_WithExistingBooks_ReturnsExpectedGetBookDtos()
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
            .Returns(Result.Success<string>(string.Empty));

        // Act:
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersQueryHandler.HandleAsync(
            _getBooksByQueryParametersQuery,
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
