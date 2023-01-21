namespace BookRentalManager.UnitTests.Application.Mappers.BookAuthorMaps;

public sealed class GetBookAuthorDtoMapperTests
{
    [Fact]
    public void Map_WithValidBookAuthor_ReturnsValidGetBookAuthorDto()
    {
        // Arrange:
        BookAuthor bookAuthor = TestFixtures.CreateDummyBookAuthor();
        var expectedGetBookAuthorDto = new GetBookAuthorDto(
            bookAuthor.Id,
            bookAuthor.FullName,
            new List<GetBookAuthorBookDto>());
        var getBookAuthorBooksDtoMapperStub = new Mock<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookAuthorBookDto>>>();
        getBookAuthorBooksDtoMapperStub
            .Setup(getBookAuthorBooksDtoMapper => getBookAuthorBooksDtoMapper.Map(It.IsAny<IReadOnlyList<Book>>()))
            .Returns(new List<GetBookAuthorBookDto>());
        var getBookAuthorDtoMapper = new GetBookAuthorDtoMapper(getBookAuthorBooksDtoMapperStub.Object);

        // Act:
        GetBookAuthorDto getBookAuthorDto = getBookAuthorDtoMapper.Map(bookAuthor);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetBookAuthorDto.Id, getBookAuthorDto.Id);
        Assert.Equal(expectedGetBookAuthorDto.FullName, getBookAuthorDto.FullName);
        Assert.Equal(expectedGetBookAuthorDto.Books, getBookAuthorDto.Books);
    }
}
