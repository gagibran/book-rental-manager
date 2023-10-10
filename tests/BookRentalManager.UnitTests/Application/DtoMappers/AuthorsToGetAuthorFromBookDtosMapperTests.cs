namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class AuthorsToGetAuthorFromBookDtosMapperTests
{
    [Fact]
    public void Map_WithValidAuthors_ReturnsValidGetAuthorFromBookDtos()
    {
        // Arrange:
        var authorsToGetAuthorFromBookDtosMapper = new AuthorsToGetAuthorFromBookDtosMapper();
        Author author = TestFixtures.CreateDummyAuthor();
        var expectedGetBookFromAuthorDto = new GetAuthorFromBookDto(author.FullName);

        // Act:
        IReadOnlyList<GetAuthorFromBookDto> getAuthorFromBookDtos = authorsToGetAuthorFromBookDtosMapper.Map(
            new List<Author> { author });

        // Assert:
        Assert.Equal(expectedGetBookFromAuthorDto.FullName, getAuthorFromBookDtos.FirstOrDefault()!.FullName);
    }
}
