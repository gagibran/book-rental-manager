namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class AuthorToAuthorCreatedDtoMapperTests
{
    [Fact]
    public void Map_WithValidAuthors_ReturnsValidGetAuthorFromBookDtos()
    {
        // Arrange:
        Author author = TestFixtures.CreateDummyAuthor();
        var expectedAuthorCreatedDto = new AuthorCreatedDto(author.Id, author.FullName.FirstName, author.FullName.LastName);
        var authorsToGetAuthorFromBookDtosMapper = new AuthorToAuthorCreatedDtoMapper();

        // Act:
        AuthorCreatedDto actualAuthorCreatedDto = authorsToGetAuthorFromBookDtosMapper.Map(author);

        // Assert:
        Assert.Equal(expectedAuthorCreatedDto.Id, actualAuthorCreatedDto.Id);
        Assert.Equal(expectedAuthorCreatedDto.FirstName, actualAuthorCreatedDto.FirstName);
        Assert.Equal(expectedAuthorCreatedDto.LastName, actualAuthorCreatedDto.LastName);
    }
}
