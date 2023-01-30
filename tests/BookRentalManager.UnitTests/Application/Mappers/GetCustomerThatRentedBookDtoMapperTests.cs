namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetCustomerThatRentedBookDtoMapperTests
{
    [Fact]
    public void Map_WithValidCustomer_ReturnGetCustomerThatRentedBookDto()
    {
        // Arrange:
        Customer customer = TestFixtures.CreateDummyCustomer();
        var getCustomerThatRentedBookDtoMapper = new GetCustomerThatRentedBookDtoMapper();
        var expectedGetCustomerThatRentedBookDto = new GetCustomerThatRentedBookDto(customer.FullName, customer.Email);

        // Act:
        GetCustomerThatRentedBookDto actualGetCustomerThatRentedBookDto = getCustomerThatRentedBookDtoMapper.Map(customer);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetCustomerThatRentedBookDto.FullName, actualGetCustomerThatRentedBookDto.FullName);
        Assert.Equal(expectedGetCustomerThatRentedBookDto.Email, actualGetCustomerThatRentedBookDto.Email);
    }

    [Fact]
    public void Map_WithoutCustomer_ReturnEmptyGetCustomerThatRentedBookDto()
    {
        // Arrange:
        var getCustomerThatRentedBookDtoMapper = new GetCustomerThatRentedBookDtoMapper();
        var expectedGetCustomerThatRentedBookDto = new GetCustomerThatRentedBookDto();

        // Act:
        GetCustomerThatRentedBookDto actualGetCustomerThatRentedBookDto = getCustomerThatRentedBookDtoMapper.Map(null);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetCustomerThatRentedBookDto.FullName, actualGetCustomerThatRentedBookDto.FullName);
        Assert.Equal(expectedGetCustomerThatRentedBookDto.Email, actualGetCustomerThatRentedBookDto.Email);
    }
}
