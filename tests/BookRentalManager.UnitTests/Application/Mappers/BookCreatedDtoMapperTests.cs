namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class BookCreatedForAuthorDtoMapperTests
{
    [Fact]
    public void Map_WithValidBook_ReturnsValidBookCreatedForAuthorDto()
    {
        // Arrange:
        var bookCreatedForAuthorDtoMapper = new BookCreatedForAuthorDtoMapper();
        Book book = TestFixtures.CreateDummyBook();
        var expectedBookCreatedForAuthorDto = new BookCreatedForAuthorDto(
            book.Id,
            book.BookTitle,
            book.Edition.EditionNumber,
            book.Isbn.IsbnValue);

        // Act:
        BookCreatedForAuthorDto bookCreatedForAuthorDto = bookCreatedForAuthorDtoMapper.Map(book);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedBookCreatedForAuthorDto.BookTitle, bookCreatedForAuthorDto.BookTitle);
        Assert.Equal(expectedBookCreatedForAuthorDto.Edition, bookCreatedForAuthorDto.Edition);
        Assert.Equal(expectedBookCreatedForAuthorDto.Isbn, bookCreatedForAuthorDto.Isbn);
    }
}
