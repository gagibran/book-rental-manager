namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetBookAuthorsForCustomerBooksDtoMapperTests
{
    private readonly GetBookAuthorsForCustomerBooksDtoMapper _getBookAuthorsForCustomerBooksDtoMapper;

    public GetBookAuthorsForCustomerBooksDtoMapperTests()
    {
        _getBookAuthorsForCustomerBooksDtoMapper = new();
    }

    [Fact]
    public void Map_WithValidBookAuthorCollection_ReturnsValidGetBookAuthorsForCustomerBooksDto()
    {
        // Arrange:
        BookAuthor bookAuthor = TestFixtures.CreateDummyBookAuthor();
        var expectedGetBookAuthorsForCustomerBooksDto = new GetBookAuthorsForCustomerBooksDto(
            bookAuthor.FullName
        );

        // Act:
        IReadOnlyList<GetBookAuthorsForCustomerBooksDto> getBookAuthorsForCustomerBooksDtos = _getBookAuthorsForCustomerBooksDtoMapper
            .Map(new List<BookAuthor> { bookAuthor });

        // Assert:
        Assert.Equal(
            expectedGetBookAuthorsForCustomerBooksDto.FullName,
            getBookAuthorsForCustomerBooksDtos.FirstOrDefault().FullName
        );
    }

    [Fact]
    public void Map_WithNullBookAuthorCollection_ReturnsEmptyBookAuthorsForCustomerBooksDtoCollection()
    {
        // Act:
        IReadOnlyList<GetBookAuthorsForCustomerBooksDto> getBookAuthorsForCustomerBooksDtos = _getBookAuthorsForCustomerBooksDtoMapper
            .Map(null);

        // Assert:
        Assert.Empty(getBookAuthorsForCustomerBooksDtos);
    }
}
