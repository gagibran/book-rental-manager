namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class AuthorToGetAuthorDtoMapperTests
{
    [Fact]
    public void Map_WithValidAuthor_ReturnsValidGetAuthorDto()
    {
        // Arrange:
        Author author = TestFixtures.CreateDummyAuthor();
        var expectedGetAuthorDto = new GetAuthorDto(
            author.Id,
            author.FullName,
            new List<GetBookFromAuthorDto>());
        var booksToGetBookFromAuthorDtosMapperStub = new Mock<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>>>();
        booksToGetBookFromAuthorDtosMapperStub
            .Setup(booksToGetBookFromAuthorDtosMapper => booksToGetBookFromAuthorDtosMapper.Map(It.IsAny<IReadOnlyList<Book>>()))
            .Returns(new List<GetBookFromAuthorDto>());
        var authorToGetAuthorDtoMapper = new AuthorToGetAuthorDtoMapper(booksToGetBookFromAuthorDtosMapperStub.Object);

        // Act:
        GetAuthorDto getAuthorDto = authorToGetAuthorDtoMapper.Map(author);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetAuthorDto.Id, getAuthorDto.Id);
        Assert.Equal(expectedGetAuthorDto.FullName, getAuthorDto.FullName);
        Assert.Equal(expectedGetAuthorDto.Books, getAuthorDto.Books);
    }
}
