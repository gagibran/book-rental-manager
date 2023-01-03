namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetBookAuthorsForCustomerBooksDtoMapperTests
{
    [Fact]
    public void Map_WithValidBooks_ReturnsValidGetBookAuthorsForCustomerBooksDto()
    {
        // Arrange:
        BookAuthor bookAuthor = TestFixtures.CreateDummyBookAuthor();
        var expectedGetBookAuthorsForCustomerBooksDto = new GetBookAuthorsForCustomerBooksDto(
            bookAuthor.FullName
        );
        var getBookAuthorsForCustomerBooksDtoMapper = new GetBookAuthorsForCustomerBooksDtoMapper();

        // Act:
        IReadOnlyList<GetBookAuthorsForCustomerBooksDto> getBookAuthorsForCustomerBooksDtos = getBookAuthorsForCustomerBooksDtoMapper
            .Map(new List<BookAuthor> { bookAuthor });

        // Assert:
        Assert.Equal(
            expectedGetBookAuthorsForCustomerBooksDto.FullName,
            getBookAuthorsForCustomerBooksDtos.FirstOrDefault().FullName
        );
    }
}
