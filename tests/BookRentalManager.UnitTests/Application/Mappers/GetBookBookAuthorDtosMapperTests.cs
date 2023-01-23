namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetBookBookAuthorDtosMapperTests
{
    [Fact]
    public void Map_WithValidBookAuthors_ReturnsValidGetBookBookAuthorDtos()
    {
        // Arrange:
        var getBookBookAuthorDtosMapper = new GetBookBookAuthorDtosMapper();
        BookAuthor bookAuthor = TestFixtures.CreateDummyBookAuthor();
        var expectedGetBookAuthorBookDto = new GetBookBookAuthorDto(bookAuthor.FullName);

        // Act:
        IReadOnlyList<GetBookBookAuthorDto> getBookBookAuthorDtos = getBookBookAuthorDtosMapper.Map(
            new List<BookAuthor> { bookAuthor });

        // Assert:
        Assert.Equal(expectedGetBookAuthorBookDto.FullName, getBookBookAuthorDtos.FirstOrDefault().FullName);
    }
}
