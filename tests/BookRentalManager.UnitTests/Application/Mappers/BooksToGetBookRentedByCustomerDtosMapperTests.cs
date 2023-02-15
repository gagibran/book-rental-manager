namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class BooksToGetBookRentedByCustomerDtosMapperTests
{
    [Fact]
    public void Map_WithValidBookCollection_ReturnsValidGetCustomerBooksDto()
    {
        // Arrange:
        var booksToGetBookRentedByCustomerDtosMapper = new BooksToGetBookRentedByCustomerDtosMapper();

        Book book = TestFixtures.CreateDummyBook();
        var expectedGetBookRentedByCustomerDto = new GetBookRentedByCustomerDto(
            book.BookTitle,
            book.Edition,
            book.Isbn);

        // Act:
        IReadOnlyList<GetBookRentedByCustomerDto> getBookRentedByCustomerDtos = booksToGetBookRentedByCustomerDtosMapper.Map(new List<Book> { book });

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetBookRentedByCustomerDto.BookTitle, getBookRentedByCustomerDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetBookRentedByCustomerDto.Edition, getBookRentedByCustomerDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetBookRentedByCustomerDto.Isbn, getBookRentedByCustomerDtos.FirstOrDefault().Isbn);
    }
}
