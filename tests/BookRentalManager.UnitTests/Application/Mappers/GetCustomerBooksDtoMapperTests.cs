namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetCustomerBooksDtoMapperTests
{
    private readonly GetCustomerBooksDtoMapper _getCustomerBooksDtoMapper;

    public GetCustomerBooksDtoMapperTests()
    {
        _getCustomerBooksDtoMapper = new();
    }

    [Fact]
    public void Map_WithValidBookCollection_ReturnsValidGetCustomerBooksDto()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var expectedGetCustomerBookDto = new GetCustomerBookDto(
            book.BookTitle,
            book.Edition,
            book.Isbn
        );

        // Act:
        IReadOnlyList<GetCustomerBookDto> getCustomerBookDtos = _getCustomerBooksDtoMapper.Map(
            new List<Book> { book }
        );

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetCustomerBookDto.BookTitle, getCustomerBookDtos.FirstOrDefault().BookTitle);
        Assert.Equal(expectedGetCustomerBookDto.Edition, getCustomerBookDtos.FirstOrDefault().Edition);
        Assert.Equal(expectedGetCustomerBookDto.Isbn, getCustomerBookDtos.FirstOrDefault().Isbn);
    }

    [Fact]
    public void Map_WithNullBookCollection_ReturnsEmptyGetCustomerBooksDto()
    {
        // Act:
        IReadOnlyList<GetCustomerBookDto> getCustomerBookDtos = _getCustomerBooksDtoMapper.Map(null);

        // Assert:
        Assert.Empty(getCustomerBookDtos);
    }
}
