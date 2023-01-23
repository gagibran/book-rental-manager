namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetBookAuthorBookDtosMapperTests
{
    [Fact]
    public void Map_WithValidBooks_ReturnsValidGetBookAuthorBookDtos()
    {
        // Arrange:
        var getBookAuthorBookDtosMapper = new GetBookAuthorBookDtosMapper();
        Book book = TestFixtures.CreateDummyBook();
        var expectedGetBookAuthorBookDto = new GetBookAuthorBookDto(
            book.BookTitle,
            book.Edition,
            book.Isbn);

        // Act:
        IReadOnlyList<GetBookAuthorBookDto> getBookAuthorBookDtos = getBookAuthorBookDtosMapper.Map(new List<Book> { book });

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetBookAuthorBookDto.BookTitle, getBookAuthorBookDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetBookAuthorBookDto.Edition, getBookAuthorBookDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetBookAuthorBookDto.Isbn, getBookAuthorBookDtos.FirstOrDefault().Isbn);
    }
}
