namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetCustomerBookDtosMapperTests
{
    [Fact]
    public void Map_WithValidBookCollection_ReturnsValidGetCustomerBooksDto()
    {
        // Arrange:
        var getCustomerBookDtosMapper = new GetCustomerBookDtosMapper();

        Book book = TestFixtures.CreateDummyBook();
        var expectedGetCustomerBookDto = new GetCustomerBookDto(
            book.BookTitle,
            book.Edition,
            book.Isbn);

        // Act:
        IReadOnlyList<GetCustomerBookDto> getCustomerBookDtos = getCustomerBookDtosMapper.Map(new List<Book> { book });

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetCustomerBookDto.BookTitle, getCustomerBookDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetCustomerBookDto.Edition, getCustomerBookDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetCustomerBookDto.Isbn, getCustomerBookDtos.FirstOrDefault().Isbn);
    }
}
