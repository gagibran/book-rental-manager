namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class BooksToGetBookFromAuthorDtosMapperTests
{
    [Fact]
    public void Map_WithValidBooks_ReturnsValidGetBookFromAuthorDtos()
    {
        // Arrange:
        var booksToGetBookFromAuthorDtosMapper = new BooksToGetBookFromAuthorDtosMapper();
        Book book = TestFixtures.CreateDummyBook();
        var expectedGetBookFromAuthorDto = new GetBookFromAuthorDto(
            book.BookTitle,
            book.Edition,
            book.Isbn);

        // Act:
        IReadOnlyList<GetBookFromAuthorDto> getBookFromAuthorDtos = booksToGetBookFromAuthorDtosMapper.Map(new List<Book> { book });

        // Assert (maybe refactor this using FluentAssertions):
        GetBookFromAuthorDto actualGetBookFromAuthorDtos = getBookFromAuthorDtos.FirstOrDefault()!;
        Assert.Equal(expectedGetBookFromAuthorDto.BookTitle, actualGetBookFromAuthorDtos.BookTitle);
        Assert.Equal(expectedGetBookFromAuthorDto.Edition, actualGetBookFromAuthorDtos.Edition);
        Assert.Equal(expectedGetBookFromAuthorDto.Isbn, actualGetBookFromAuthorDtos.Isbn);
    }
}
