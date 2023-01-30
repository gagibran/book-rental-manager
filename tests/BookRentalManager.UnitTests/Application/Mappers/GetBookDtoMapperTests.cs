namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetBookDtoMapperTests
{
    [Fact]
    public void Map_withValidBook_ReturnsValidGetBookDto()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var getAuthorFromBookDtosMapperStub = new Mock<IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>>>();
        var getCustomerThatRentedBookDtoMapperStub = new Mock<IMapper<Customer, GetCustomerThatRentedBookDto>>();
        getAuthorFromBookDtosMapperStub
            .Setup(getAuthorFromBookDtosMapper => getAuthorFromBookDtosMapper.Map(It.IsAny<IReadOnlyList<Author>>()))
            .Returns(new List<GetAuthorFromBookDto>());
        getCustomerThatRentedBookDtoMapperStub
            .Setup(getCustomerThatRentedBookDtoMapper => getCustomerThatRentedBookDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(new GetCustomerThatRentedBookDto());
        var getBookDtoMapper = new GetBookDtoMapper(getAuthorFromBookDtosMapperStub.Object, getCustomerThatRentedBookDtoMapperStub.Object);
        var expectedGetBookDto = new GetBookDto(
            book.Id,
            book.BookTitle,
            new List<GetAuthorFromBookDto>(),
            book.Edition,
            book.Isbn,
            book.IsAvailable,
            new GetCustomerThatRentedBookDto());

        // Act:
        GetBookDto actualGetBookDto = getBookDtoMapper.Map(book);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetBookDto.BookTitle, actualGetBookDto.BookTitle);
        Assert.Equal(expectedGetBookDto.Authors, actualGetBookDto.Authors);
        Assert.Equal(expectedGetBookDto.Edition, actualGetBookDto.Edition);
        Assert.Equal(expectedGetBookDto.Isbn, actualGetBookDto.Isbn);
        Assert.Equal(expectedGetBookDto.IsAvailable, actualGetBookDto.IsAvailable);
        Assert.Equal(expectedGetBookDto.RentedBy.FullName, actualGetBookDto.RentedBy.FullName);
        Assert.Equal(expectedGetBookDto.RentedBy.Email, actualGetBookDto.RentedBy.Email);
    }
}
