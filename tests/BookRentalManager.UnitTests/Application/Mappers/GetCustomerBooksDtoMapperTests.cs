namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetCustomerBooksDtoMapperTests
{
    private readonly Mock<IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookAuthorsForCustomerBooksDto>>> _getBookAuthorsForCustomerBooksDtoMapperStub;
    private readonly GetCustomerBooksDtoMapper _getCustomerBooksDtoMapper;

    public GetCustomerBooksDtoMapperTests()
    {
        _getBookAuthorsForCustomerBooksDtoMapperStub = new();
        _getBookAuthorsForCustomerBooksDtoMapperStub
            .Setup(getBookAuthorsForCustomerBooksDtoMapper => getBookAuthorsForCustomerBooksDtoMapper
                .Map(It.IsAny<IReadOnlyList<BookAuthor>>()));
        _getCustomerBooksDtoMapper = new(_getBookAuthorsForCustomerBooksDtoMapperStub.Object);
    }

    [Fact]
    public void Map_WithValidBookCollection_ReturnsValidGetCustomerBooksDto()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var expectedGetCustomerBooksDto = new GetCustomerBooksDto(
            book.BookTitle,
            null,
            book.Edition,
            book.Isbn
        );

        // Act:
        IReadOnlyList<GetCustomerBooksDto> getCustomerBooksDtos = _getCustomerBooksDtoMapper
            .Map(new List<Book> { book });

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetCustomerBooksDto.BookTitle, getCustomerBooksDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetCustomerBooksDto.BookAuthors, getCustomerBooksDtos.FirstOrDefault().BookAuthors);
        Assert.Equal(expectedGetCustomerBooksDto.Edition, getCustomerBooksDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetCustomerBooksDto.Isbn, getCustomerBooksDtos.FirstOrDefault().Isbn);
    }

    [Fact]
    public void Map_WithNullBookCollection_ReturnsEmptyGetCustomerBooksDto()
    {
        // Act:
        IReadOnlyList<GetCustomerBooksDto> getCustomerBooksDtos = _getCustomerBooksDtoMapper.Map(null);

        // Assert:
        Assert.Empty(getCustomerBooksDtos);
    }
}
