namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetCustomerDtoMapperTests
{
    [Fact]
    public void Map_WithValidCustomer_ReturnsValidGetCustomerDto()
    {
        // Arrange:
        Customer customer = TestFixtures.CreateDummyCustomer();
        var expectedGetCustomerDto = new GetCustomerDto(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.PhoneNumber,
            new List<GetCustomerBooksDto>(),
            customer.CustomerStatus,
            customer.CustomerPoints
        );
        var getCustomerBooksDtoMapperStub = new Mock<IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBooksDto>>>();
        getCustomerBooksDtoMapperStub
            .Setup(getCustomerBooksDto => getCustomerBooksDto.Map(It.IsAny<IReadOnlyList<Book>>()))
            .Returns(new List<GetCustomerBooksDto>());
        var getCustomerDtoMapper = new GetCustomerDtoMapper(getCustomerBooksDtoMapperStub.Object);

        // Act:
        GetCustomerDto getCustomerDto = getCustomerDtoMapper.Map(customer);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetCustomerDto.Id, getCustomerDto.Id);
        Assert.Equal(expectedGetCustomerDto.FullName, getCustomerDto.FullName);
        Assert.Equal(expectedGetCustomerDto.Email, getCustomerDto.Email);
        Assert.Equal(expectedGetCustomerDto.PhoneNumber, getCustomerDto.PhoneNumber);
        Assert.Equal(expectedGetCustomerDto.Books, getCustomerDto.Books);
        Assert.Equal(expectedGetCustomerDto.CustomerStatus, getCustomerDto.CustomerStatus);
        Assert.Equal(expectedGetCustomerDto.CustomerPoints, getCustomerDto.CustomerPoints);
    }
}