namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetBookAuthorBooksDtoMapperTests
{
    private readonly GetBookAuthorBooksDtoMapper _getBookAuthorBooksDtoMapper;

    public GetBookAuthorBooksDtoMapperTests()
    {
        _getBookAuthorBooksDtoMapper = new();
    }

    [Fact]
    public void Map_WithValidBookCollection_ReturnsValidGetBookAuthorBooksDto()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var expectedGetBookAuthorBookDto = new GetBookAuthorBookDto(
            book.BookTitle,
            book.Edition,
            book.Isbn
        );

        // Act:
        IReadOnlyList<GetBookAuthorBookDto> getBookAuthorBookDtos = _getBookAuthorBooksDtoMapper.Map(
            new List<Book> { book }
        );

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetBookAuthorBookDto.BookTitle, getBookAuthorBookDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetBookAuthorBookDto.Edition, getBookAuthorBookDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetBookAuthorBookDto.Isbn, getBookAuthorBookDtos.FirstOrDefault().Isbn);
    }

    [Fact]
    public void Map_WithNullBookCollection_ReturnsEmptyGetBookAuthorBooksDto()
    {
        // Act:
        IReadOnlyList<GetBookAuthorBookDto> getBookAuthorBookDtos = _getBookAuthorBooksDtoMapper.Map(null);

        // Assert:
        Assert.Empty(getBookAuthorBookDtos);
    }
}
