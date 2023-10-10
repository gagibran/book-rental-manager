namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class BookToGetBookDtoMapperTests
{
    [Fact]
    public void Map_withValidBook_ReturnsValidGetBookDto()
    {
        // Arrange:
        Book book = TestFixtures.CreateDummyBook();
        var authorsToGetAuthorFromBookDtosMapperStub = new Mock<IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>>>();
        var customerToGetCustomerThatRentedBookDtoMapperStub = new Mock<IMapper<Customer, GetCustomerThatRentedBookDto>>();
        authorsToGetAuthorFromBookDtosMapperStub
            .Setup(authorsToGetAuthorFromBookDtosMapper => authorsToGetAuthorFromBookDtosMapper.Map(It.IsAny<IReadOnlyList<Author>>()))
            .Returns(new List<GetAuthorFromBookDto>());
        customerToGetCustomerThatRentedBookDtoMapperStub
            .Setup(customerToGetCustomerThatRentedBookDtoMapper => customerToGetCustomerThatRentedBookDtoMapper.Map(It.IsAny<Customer>()))
            .Returns(new GetCustomerThatRentedBookDto());
        var bookToGetBookDtoMapper = new BookToGetBookDtoMapper(
            authorsToGetAuthorFromBookDtosMapperStub.Object,
            customerToGetCustomerThatRentedBookDtoMapperStub.Object!);
        var expectedGetBookDto = new GetBookDto(
            book.Id,
            book.BookTitle,
            new List<GetAuthorFromBookDto>(),
            book.Edition,
            book.Isbn,
            book.RentedAt,
            book.DueDate,
            new GetCustomerThatRentedBookDto());

        // Act:
        GetBookDto actualGetBookDto = bookToGetBookDtoMapper.Map(book);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetBookDto.BookTitle, actualGetBookDto.BookTitle);
        Assert.Equal(expectedGetBookDto.Authors, actualGetBookDto.Authors);
        Assert.Equal(expectedGetBookDto.Edition, actualGetBookDto.Edition);
        Assert.Equal(expectedGetBookDto.Isbn, actualGetBookDto.Isbn);
        Assert.Equal(expectedGetBookDto.RentedAt, actualGetBookDto.RentedAt);
        Assert.Equal(expectedGetBookDto.DueDate, actualGetBookDto.DueDate);
        Assert.Equal(expectedGetBookDto.RentedBy.FullName, actualGetBookDto.RentedBy.FullName);
        Assert.Equal(expectedGetBookDto.RentedBy.Email, actualGetBookDto.RentedBy.Email);
    }
}
