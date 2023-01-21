namespace BookRentalManager.UnitTests.Application.Mappers.BookMaps;

public sealed class GetRentedByDtoMapperTests
{
    [Fact]
    public void Map_WithValidCustomer_ReturnGetRentedByDto()
    {
        // Arrange:
        Customer customer = TestFixtures.CreateDummyCustomer();
        var getRentedByDtoMapper = new GetRentedByDtoMapper();
        var expectedGetRentedByDto = new GetRentedByDto(customer.FullName, customer.Email);

        // Act:
        GetRentedByDto actualGetRentedByDto = getRentedByDtoMapper.Map(customer);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetRentedByDto.FullName, actualGetRentedByDto.FullName);
        Assert.Equal(expectedGetRentedByDto.Email, actualGetRentedByDto.Email);
    }

    [Fact]
    public void Map_WithoutCustomer_ReturnEmptyGetRentedByDto()
    {
        // Arrange:
        var getRentedByDtoMapper = new GetRentedByDtoMapper();
        var expectedGetRentedByDto = new GetRentedByDto();

        // Act:
        GetRentedByDto actualGetRentedByDto = getRentedByDtoMapper.Map(null);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetRentedByDto.FullName, actualGetRentedByDto.FullName);
        Assert.Equal(expectedGetRentedByDto.Email, actualGetRentedByDto.Email);
    }
}
