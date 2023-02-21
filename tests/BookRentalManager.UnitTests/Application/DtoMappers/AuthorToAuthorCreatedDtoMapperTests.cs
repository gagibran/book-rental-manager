namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class AuthorToAuthorCreatedDtoMapperTests
{
    [Fact]
    public void Map_WithValidAuthors_ReturnsValidGetAuthorFromBookDtos()
    {
        // Arrange:
        Author author = TestFixtures.CreateDummyAuthor();
        var expectedAuthorCreatedDto = new AuthorCreatedDto(author.Id, author.FullName.CompleteName);
        var authorsToGetAuthorFromBookDtosMapper = new AuthorToAuthorCreatedDtoMapper();

        // Act:
        AuthorCreatedDto actualAuthorCreatedDto = authorsToGetAuthorFromBookDtosMapper.Map(author);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedAuthorCreatedDto.Id, actualAuthorCreatedDto.Id);
        Assert.Equal(expectedAuthorCreatedDto.FullName, actualAuthorCreatedDto.FullName);
    }
}
