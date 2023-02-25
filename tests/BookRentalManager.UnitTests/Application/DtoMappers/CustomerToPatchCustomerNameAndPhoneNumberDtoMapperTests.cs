namespace BookRentalManager.UnitTests.Application.DtoMappers;

public sealed class CustomerToPatchCustomerNameAndPhoneNumberDtoMapperTests
{
    [Fact]
    public void Map_WithValidCustomer_ReturnsValidPatchCustomerNameAndPhoneNumberDto()
    {
        // Arrange:
        var customerToPatchCustomerNameAndPhoneNumberDtoMapper = new CustomerToPatchCustomerNameAndPhoneNumberDtoMapper();
        Customer customer = TestFixtures.CreateDummyCustomer();
        var expectedPatchCustomerNameAndPhoneNumberDto = new PatchCustomerNameAndPhoneNumberDto(
            customer.FullName.FirstName,
            customer.FullName.LastName,
            customer.PhoneNumber.AreaCode,
            customer.PhoneNumber.PrefixAndLineNumber);

        // Act:
        PatchCustomerNameAndPhoneNumberDto actualPatchCustomerNameAndPhoneNumberDto = customerToPatchCustomerNameAndPhoneNumberDtoMapper.Map(customer);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedPatchCustomerNameAndPhoneNumberDto.FirstName, actualPatchCustomerNameAndPhoneNumberDto.FirstName);
        Assert.Equal(expectedPatchCustomerNameAndPhoneNumberDto.LastName, actualPatchCustomerNameAndPhoneNumberDto.LastName);
        Assert.Equal(expectedPatchCustomerNameAndPhoneNumberDto.AreaCode, actualPatchCustomerNameAndPhoneNumberDto.AreaCode);
        Assert.Equal(expectedPatchCustomerNameAndPhoneNumberDto.PrefixAndLineNumber, actualPatchCustomerNameAndPhoneNumberDto.PrefixAndLineNumber);
    }
}
