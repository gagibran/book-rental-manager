namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetBookAuthorBooksDtoMapperTests
{
    [Fact]
    public void Map_WithValidBookCollection_ReturnsValidGetBookAuthorBooksDto()
    {
        // Arrange:
        var getBookAuthorBooksDtoMapper = new GetBookAuthorBooksDtoMapper();
        Book book = TestFixtures.CreateDummyBook();
        var expectedGetBookAuthorBookDto = new GetBookAuthorBookDto(
            book.BookTitle,
            book.Edition,
            book.Isbn);

        // Act:
        IReadOnlyList<GetBookAuthorBookDto> getBookAuthorBookDtos = getBookAuthorBooksDtoMapper.Map(new List<Book> { book });

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetBookAuthorBookDto.BookTitle, getBookAuthorBookDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetBookAuthorBookDto.Edition, getBookAuthorBookDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetBookAuthorBookDto.Isbn, getBookAuthorBookDtos.FirstOrDefault().Isbn);
    }
}
