namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class CustomerToGetCustomerThatRentedBookDtoMapperTests
{
    [Fact]
    public void Map_WithValidCustomer_ReturnGetCustomerThatRentedBookDto()
    {
        // Arrange:
        Customer customer = TestFixtures.CreateDummyCustomer();
        var customerToGetCustomerThatRentedBookDtoMapper = new CustomerToGetCustomerThatRentedBookDtoMapper();
        var expectedGetCustomerThatRentedBookDto = new GetCustomerThatRentedBookDto(customer.FullName, customer.Email);

        // Act:
        GetCustomerThatRentedBookDto actualGetCustomerThatRentedBookDto = customerToGetCustomerThatRentedBookDtoMapper.Map(customer);

        // Assert:
        Assert.Equal(expectedGetCustomerThatRentedBookDto.FullName, actualGetCustomerThatRentedBookDto.FullName);
        Assert.Equal(expectedGetCustomerThatRentedBookDto.Email, actualGetCustomerThatRentedBookDto.Email);
    }

    [Fact]
    public void Map_WithoutCustomer_ReturnEmptyGetCustomerThatRentedBookDto()
    {
        // Arrange:
        var customerToGetCustomerThatRentedBookDtoMapper = new CustomerToGetCustomerThatRentedBookDtoMapper();
        var expectedGetCustomerThatRentedBookDto = new GetCustomerThatRentedBookDto();

        // Act:
        GetCustomerThatRentedBookDto actualGetCustomerThatRentedBookDto = customerToGetCustomerThatRentedBookDtoMapper.Map(null);

        // Assert:
        Assert.Equal(expectedGetCustomerThatRentedBookDto.FullName, actualGetCustomerThatRentedBookDto.FullName);
        Assert.Equal(expectedGetCustomerThatRentedBookDto.Email, actualGetCustomerThatRentedBookDto.Email);
    }
}
