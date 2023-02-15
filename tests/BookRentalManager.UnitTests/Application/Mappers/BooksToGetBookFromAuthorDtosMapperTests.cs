namespace BookRentalManager.UnitTests.Application.Mappers;

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
        Assert.Equal(expectedGetBookFromAuthorDto.BookTitle, getBookFromAuthorDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetBookFromAuthorDto.Edition, getBookFromAuthorDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetBookFromAuthorDto.Isbn, getBookFromAuthorDtos.FirstOrDefault().Isbn);
    }
}
