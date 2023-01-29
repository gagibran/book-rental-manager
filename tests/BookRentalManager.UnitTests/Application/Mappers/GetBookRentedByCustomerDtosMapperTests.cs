namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetBookRentedByCustomerDtosMapperTests
{
    [Fact]
    public void Map_WithValidBookCollection_ReturnsValidGetCustomerBooksDto()
    {
        // Arrange:
        var getBookRentedByCustomerDtosMapper = new GetBookRentedByCustomerDtosMapper();

        Book book = TestFixtures.CreateDummyBook();
        var expectedGetBookRentedByCustomerDto = new GetBookRentedByCustomerDto(
            book.BookTitle,
            book.Edition,
            book.Isbn);

        // Act:
        IReadOnlyList<GetBookRentedByCustomerDto> getBookRentedByCustomerDtos = getBookRentedByCustomerDtosMapper.Map(new List<Book> { book });

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetBookRentedByCustomerDto.BookTitle, getBookRentedByCustomerDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetBookRentedByCustomerDto.Edition, getBookRentedByCustomerDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetBookRentedByCustomerDto.Isbn, getBookRentedByCustomerDtos.FirstOrDefault().Isbn);
    }
}
