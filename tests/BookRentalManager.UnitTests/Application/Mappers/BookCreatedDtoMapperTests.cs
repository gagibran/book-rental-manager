namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class BookForAuthorCreatedDtoMapperTests
{
    [Fact]
    public void Map_WithValidBook_ReturnsValidBookForAuthorCreatedDto()
    {
        // Arrange:
        var bookForAuthorCreatedDtoMapper = new BookForAuthorCreatedDtoMapper();
        Book book = TestFixtures.CreateDummyBook();
        var expectedBookForAuthorCreatedDto = new BookForAuthorCreatedDto(
            book.Id,
            book.BookTitle,
            book.Edition.EditionNumber,
            book.Isbn.IsbnValue);

        // Act:
        BookForAuthorCreatedDto bookForAuthorCreatedDto = bookForAuthorCreatedDtoMapper.Map(book);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedBookForAuthorCreatedDto.BookTitle, bookForAuthorCreatedDto.BookTitle);
        Assert.Equal(expectedBookForAuthorCreatedDto.Edition, bookForAuthorCreatedDto.Edition);
        Assert.Equal(expectedBookForAuthorCreatedDto.Isbn, bookForAuthorCreatedDto.Isbn);
    }
}
