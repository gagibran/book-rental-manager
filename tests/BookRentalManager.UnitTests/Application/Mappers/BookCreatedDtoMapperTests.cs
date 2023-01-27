namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class BookCreatedDtoMapperTests
{
    [Fact]
    public void Map_WithValidBook_ReturnsValidBookCreatedDto()
    {
        // Arrange:
        var bookCreatedDtoMapper = new BookCreatedDtoMapper();
        Book book = TestFixtures.CreateDummyBook();
        var expectedBookCreatedDto = new BookCreatedDto(
            book.Id,
            book.BookTitle,
            book.Edition.EditionNumber,
            book.Isbn.IsbnValue);

        // Act:
        BookCreatedDto bookCreatedDto = bookCreatedDtoMapper.Map(book);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedBookCreatedDto.BookTitle, bookCreatedDto.BookTitle);
        Assert.Equal(expectedBookCreatedDto.Edition, bookCreatedDto.Edition);
        Assert.Equal(expectedBookCreatedDto.Isbn, bookCreatedDto.Isbn);
    }
}
