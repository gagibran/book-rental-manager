namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class BooksToGetBookRentedByCustomerDtosMapperTests
{
    [Fact]
    public void Map_WithValidBookCollection_ReturnsValidGetCustomerBooksDto()
    {
        // Arrange:
        var booksToGetBookRentedByCustomerDtosMapper = new BooksToGetBookRentedByCustomerDtosMapper();

        Book book = TestFixtures.CreateDummyBook();
        Customer customer = TestFixtures.CreateDummyCustomer();
        customer.RentBook(book);
        var expectedGetBookRentedByCustomerDto = new GetBookRentedByCustomerDto(
            book.BookTitle,
            book.Edition,
            book.Isbn,
            book.RentedAt!.Value,
            book.DueDate!.Value);

        // Act:
        IReadOnlyList<GetBookRentedByCustomerDto> getBookRentedByCustomerDtos = booksToGetBookRentedByCustomerDtosMapper.Map(
            new List<Book> { book });

        // Assert (maybe refactor this using FluentAssertions):
        GetBookRentedByCustomerDto actualGetBookRentedByCustomerDtos = getBookRentedByCustomerDtos.FirstOrDefault()!;
        Assert.Equal(expectedGetBookRentedByCustomerDto.BookTitle, actualGetBookRentedByCustomerDtos.BookTitle);
        Assert.Equal(expectedGetBookRentedByCustomerDto.Edition, actualGetBookRentedByCustomerDtos.Edition);
        Assert.Equal(expectedGetBookRentedByCustomerDto.Isbn, actualGetBookRentedByCustomerDtos.Isbn);
    }
}
