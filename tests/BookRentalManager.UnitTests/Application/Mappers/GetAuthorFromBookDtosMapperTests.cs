namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetAuthorFromBookDtosMapperTests
{
    [Fact]
    public void Map_WithValidAuthors_ReturnsValidGetAuthorFromBookDtos()
    {
        // Arrange:
        var getAuthorFromBookDtosMapper = new GetAuthorFromBookDtosMapper();
        Author author = TestFixtures.CreateDummyAuthor();
        var expectedGetBookFromAuthorDto = new GetAuthorFromBookDto(author.FullName);

        // Act:
        IReadOnlyList<GetAuthorFromBookDto> getAuthorFromBookDtos = getAuthorFromBookDtosMapper.Map(
            new List<Author> { author });

        // Assert:
        Assert.Equal(expectedGetBookFromAuthorDto.FullName, getAuthorFromBookDtos.FirstOrDefault().FullName);
    }
}
