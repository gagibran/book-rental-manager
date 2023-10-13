namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class BookToBookCreatedDtoMapperTests
{
    [Fact]
    public void Map_WithValidBook_ReturnsValidBookCreatedDto()
    {
        // Arrange:
        var bookToBookCreatedDtoMapper = new BookToBookCreatedDtoMapper();
        Book book = TestFixtures.CreateDummyBook();
        var expectedBookCreatedDto = new BookCreatedDto(
            book.Id,
            book.BookTitle,
            book.Edition.EditionNumber,
            book.Isbn.IsbnValue);

        // Act:
        BookCreatedDto bookCreatedDto = bookToBookCreatedDtoMapper.Map(book);

        // Assert:
        Assert.Equal(expectedBookCreatedDto.BookTitle, bookCreatedDto.BookTitle);
        Assert.Equal(expectedBookCreatedDto.Edition, bookCreatedDto.Edition);
        Assert.Equal(expectedBookCreatedDto.Isbn, bookCreatedDto.Isbn);
    }
}
