namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class BookToGetBookDtoMapperTests
{
    private readonly Book _book;
    private readonly Mock<IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>>> _authorsToGetAuthorFromBookDtosMapperStub;
    private readonly BookToGetBookDtoMapper _bookToGetBookDtoMapper;

    public BookToGetBookDtoMapperTests()
    {
        _book = TestFixtures.CreateDummyBook();
        _authorsToGetAuthorFromBookDtosMapperStub = new();
        _bookToGetBookDtoMapper = new(_authorsToGetAuthorFromBookDtosMapperStub.Object);
        _authorsToGetAuthorFromBookDtosMapperStub
            .Setup(authorsToGetAuthorFromBookDtosMapper => authorsToGetAuthorFromBookDtosMapper.Map(It.IsAny<IReadOnlyList<Author>>()))
            .Returns(new List<GetAuthorFromBookDto>());
    }

    [Fact]
    public void Map_WithBookNotBeingRented_ReturnsGetBookDtoWithNullRentedBy()
    {
        // Arrange:
        var expectedGetBookDto = new GetBookDto(
            _book.Id,
            _book.BookTitle,
            new List<GetAuthorFromBookDto>(),
            _book.Edition,
            _book.Isbn,
            _book.RentedAt,
            _book.DueDate,
            null);

        // Act:
        GetBookDto actualGetBookDto = _bookToGetBookDtoMapper.Map(_book);

        // Assert:
        Assert.Equal(expectedGetBookDto.BookTitle, actualGetBookDto.BookTitle);
        Assert.Equal(expectedGetBookDto.Authors, actualGetBookDto.Authors);
        Assert.Equal(expectedGetBookDto.Edition, actualGetBookDto.Edition);
        Assert.Equal(expectedGetBookDto.Isbn, actualGetBookDto.Isbn);
        Assert.Equal(expectedGetBookDto.RentedAt, actualGetBookDto.RentedAt);
        Assert.Equal(expectedGetBookDto.DueDate, actualGetBookDto.DueDate);
        Assert.Null(expectedGetBookDto.RentedBy);
    }

    [Fact]
    public void Map_WithValidBook_ReturnsValidGetBookDto()
    {
        // Arrange:
        Customer customer = TestFixtures.CreateDummyCustomer();
        _book.SetRentedBy(customer);
        var expectedGetBookDto = new GetBookDto(
            _book.Id,
            _book.BookTitle,
            new List<GetAuthorFromBookDto>(),
            _book.Edition,
            _book.Isbn,
            _book.RentedAt,
            _book.DueDate,
            new GetCustomerThatRentedBookDto(customer));

        // Act:
        GetBookDto actualGetBookDto = _bookToGetBookDtoMapper.Map(_book);

        // Assert:
        Assert.Equal(expectedGetBookDto.BookTitle, actualGetBookDto.BookTitle);
        Assert.Equal(expectedGetBookDto.Authors, actualGetBookDto.Authors);
        Assert.Equal(expectedGetBookDto.Edition, actualGetBookDto.Edition);
        Assert.Equal(expectedGetBookDto.Isbn, actualGetBookDto.Isbn);
        Assert.Equal(expectedGetBookDto.RentedAt, actualGetBookDto.RentedAt);
        Assert.Equal(expectedGetBookDto.DueDate, actualGetBookDto.DueDate);
        Assert.Equal(expectedGetBookDto.RentedBy!.FullName, actualGetBookDto.RentedBy!.FullName);
        Assert.Equal(expectedGetBookDto.RentedBy.Email, actualGetBookDto.RentedBy.Email);
    }
}
