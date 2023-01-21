namespace BookRentalManager.UnitTests.Application.Mappers.CustomerMaps;

public sealed class GetCustomerBooksDtoMapperTests
{
    [Fact]
    public void Map_WithValidBookCollection_ReturnsValidGetCustomerBooksDto()
    {
        // Arrange:
        var getCustomerBooksDtoMapper = new GetCustomerBooksDtoMapper();

        Book book = TestFixtures.CreateDummyBook();
        var expectedGetCustomerBookDto = new GetCustomerBookDto(
            book.BookTitle,
            book.Edition,
            book.Isbn);

        // Act:
        IReadOnlyList<GetCustomerBookDto> getCustomerBookDtos = getCustomerBooksDtoMapper.Map(new List<Book> { book });

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetCustomerBookDto.BookTitle, getCustomerBookDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetCustomerBookDto.Edition, getCustomerBookDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetCustomerBookDto.Isbn, getCustomerBookDtos.FirstOrDefault().Isbn);
    }
}
