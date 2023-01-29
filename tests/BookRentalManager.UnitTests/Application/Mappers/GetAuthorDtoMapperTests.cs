namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetAuthorDtoMapperTests
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
        var getBookFromAuthorDtosMapperStub = new Mock<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>>>();
        getBookFromAuthorDtosMapperStub
            .Setup(getBookFromAuthorDtosMapper => getBookFromAuthorDtosMapper.Map(It.IsAny<IReadOnlyList<Book>>()))
            .Returns(new List<GetBookFromAuthorDto>());
        var getAuthorDtoMapper = new GetAuthorDtoMapper(getBookFromAuthorDtosMapperStub.Object);

        // Act:
        GetAuthorDto getAuthorDto = getAuthorDtoMapper.Map(author);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetAuthorDto.Id, getAuthorDto.Id);
        Assert.Equal(expectedGetAuthorDto.FullName, getAuthorDto.FullName);
        Assert.Equal(expectedGetAuthorDto.Books, getAuthorDto.Books);
    }
}
