namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class BookToPatchBookTitleEditionAndIsbnByIdDtoMapperTests
{
    [Fact]
    public void Map_WithValidBook_ReturnsValidPatchBookTitleEditionAndIsbnByIdDto()
    {
        // Arrange:
        var bookToPatchBookTitleEditionAndIsbnByIdDtoMapper = new BookToPatchBookTitleEditionAndIsbnByIdDtoMapper();
        Book book = TestFixtures.CreateDummyBook();
        var expectedPatchBookTitleEditionAndIsbnByIdDto = new PatchBookTitleEditionAndIsbnByIdDto(
            book.BookTitle,
            book.Edition.EditionNumber,
            book.Isbn.IsbnValue);

        // Act:
        PatchBookTitleEditionAndIsbnByIdDto actualPatchBookTitleEditionAndIsbnByIdDto = bookToPatchBookTitleEditionAndIsbnByIdDtoMapper.Map(book);

        // Assert:
        Assert.Equal(expectedPatchBookTitleEditionAndIsbnByIdDto.BookTitle, actualPatchBookTitleEditionAndIsbnByIdDto.BookTitle);
        Assert.Equal(expectedPatchBookTitleEditionAndIsbnByIdDto.Edition, actualPatchBookTitleEditionAndIsbnByIdDto.Edition);
        Assert.Equal(expectedPatchBookTitleEditionAndIsbnByIdDto.Isbn, actualPatchBookTitleEditionAndIsbnByIdDto.Isbn);
    }
}
