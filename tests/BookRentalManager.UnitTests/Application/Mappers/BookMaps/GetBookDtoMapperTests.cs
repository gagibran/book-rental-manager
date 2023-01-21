namespace BookRentalManager.UnitTests.Application.Mappers.BookMaps;

public sealed class GetBookDtoMapperTests
{
    [Fact]
    public void Map_withValidBook_ReturnsValidGetBookDto()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var getBookBookAuthorDtosMapperStub = new Mock<IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookBookAuthorDto>>>();
        var getRentedByDtoMapperStub = new Mock<IMapper<Customer, GetRentedByDto>>();
        getBookBookAuthorDtosMapperStub
            .Setup(getBookBookAuthorDtosMapper => getBookBookAuthorDtosMapper.Map(It.IsAny<IReadOnlyList<BookAuthor>>()))
            .Returns(new List<GetBookBookAuthorDto>());
        getRentedByDtoMapperStub
            .Setup(getRentedByDtoMapper => getRentedByDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(new GetRentedByDto());
        var getBookDtoMapper = new GetBookDtoMapper(getBookBookAuthorDtosMapperStub.Object, getRentedByDtoMapperStub.Object);
        var expectedGetBookDto = new GetBookDto(
            book.BookTitle,
            new List<GetBookBookAuthorDto>(),
            book.Edition,
            book.Isbn,
            book.IsAvailable,
            new GetRentedByDto());

        // Act:
        GetBookDto actualGetBookDto = getBookDtoMapper.Map(book);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetBookDto.BookTitle, actualGetBookDto.BookTitle);
        Assert.Equal(expectedGetBookDto.BookAuthors, actualGetBookDto.BookAuthors);
        Assert.Equal(expectedGetBookDto.Edition, actualGetBookDto.Edition);
        Assert.Equal(expectedGetBookDto.Isbn, actualGetBookDto.Isbn);
        Assert.Equal(expectedGetBookDto.IsAvailable, actualGetBookDto.IsAvailable);
        Assert.Equal(expectedGetBookDto.RentedBy.FullName, actualGetBookDto.RentedBy.FullName);
        Assert.Equal(expectedGetBookDto.RentedBy.Email, actualGetBookDto.RentedBy.Email);
    }
}
