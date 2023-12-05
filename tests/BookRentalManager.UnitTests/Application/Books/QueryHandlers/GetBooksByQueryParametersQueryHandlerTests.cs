namespace BookRentalManager.UnitTests.Application.Books.QueryHandlers;

public sealed class GetBooksByQueryParametersQueryHandlerTests
{
    private readonly GetBooksByQueryParametersQuery _getBooksByQueryParametersQuery;
    private readonly Mock<IRepository<Book>> _bookRepositoryStub;
    private readonly Mock<ISortParametersMapper> _sortParametersMapperStub;
    private readonly GetBooksByQueryParametersQueryHandler _getBooksByQueryParametersQueryHandler;

    public GetBooksByQueryParametersQueryHandlerTests()
    {
        _getBooksByQueryParametersQuery = new(
            It.IsAny<int>(),
            It.IsAny<int>(),
            string.Empty,
            It.IsAny<string>());
        _bookRepositoryStub = new();
        _sortParametersMapperStub = new();
        _getBooksByQueryParametersQueryHandler = new GetBooksByQueryParametersQueryHandler(
            _bookRepositoryStub.Object,
            _sortParametersMapperStub.Object);
    }

    [Fact]
    public async Task HandleAsync_WithExistingBooks_ReturnsExpectedGetBookDtos()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var paginatedBooks = new PaginatedList<Book>(
            [book],
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>());
        var getBookDto = new GetBookDto(
            book.Id,
            book.BookTitle,
            new List<GetAuthorFromBookDto>(),
            book.Edition,
            book.Isbn,
            book.RentedAt,
            book.DueDate,
            null);
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
        Result<PaginatedList<GetBookDto>> handlerResult = await _getBooksByQueryParametersQueryHandler.HandleAsync(
            _getBooksByQueryParametersQuery,
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
