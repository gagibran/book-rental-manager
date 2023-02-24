namespace CustomerRentalManager.UnitTests.Application.SortParametersMappers;

public sealed class CustomerSortParametersMapperTests
{
    [Fact]
    public void Map_WithInputData_ReturnsExpectedOutputData()
    {
        // Arrange:
        var expectedQuery = "Email.EmailAddressDesc,PhoneNumber.AreaCode,PhoneNumber.PrefixAndLineNumber,CustomerPointsDesc,CustomerStatus.CustomerTypeDesc,FullName.FirstName,FullName.LastName";
        var customerSortParametersMapper = new CustomerSortParametersMapper();
        var customerSortParameters = new CustomerSortParameters("EmailDesc,PhoneNumber,CustomerPointsDesc,CustomerStatusDesc,FullName");

        // Act:
        Result<string> actualResult = customerSortParametersMapper.Map(customerSortParameters);

        // Assert:
        Assert.Equal(expectedQuery, actualResult.Value);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("NotExistingProperty,  , AnotherNotExistingOne", "NotExistingProperty")]
    [InlineData("NotExistingProperty,PhoneNumberDesc, AnotherNotExistingOne   ,CustomerPoints", "NotExistingProperty")]
    public void Map_WithInvalidInputData_ReturnsErrorMessage(string propertyNamesSeparatedByComma, string faultyProperty)
    {
        // Arrange:
        var expectedErrorMessage = $"The property '{faultyProperty}' does not exist for '{nameof(GetCustomerDto)}'.";
        var customerSortParametersMapper = new CustomerSortParametersMapper();
        var customerSortParameters = new CustomerSortParameters(propertyNamesSeparatedByComma);

        // Act:
        Result<string> actualResult = customerSortParametersMapper.Map(customerSortParameters);

        // Assert:
        Assert.Equal(expectedErrorMessage, actualResult.ErrorMessage);
    }
}
