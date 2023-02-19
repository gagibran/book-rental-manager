namespace CustomerRentalManager.UnitTests.Application.SortParametersMappers;

public sealed class CustomerSortParametersMapperTests
{
    [Theory]
    [InlineData("EmailDesc,PhoneNumber,CustomerPointsDesc,CustomerStatusDesc,FullName",
        "Email.EmailAddressDesc,PhoneNumber.CompletePhoneNumber,CustomerPointsDesc,CustomerStatus.CustomerTypeDesc,FullName.CompleteName")]
    public void Map_WithInputData_ReturnsExpectedOutputData(string propertyNamesSeparatedByComma, string expectedResult)
    {
        // Arrange:
        var customerSortParametersMapper = new CustomerSortParametersMapper();
        var customerSortParameters = new CustomerSortParameters(propertyNamesSeparatedByComma);

        // Act:
        Result<string> actualResult = customerSortParametersMapper.Map(customerSortParameters);

        // Assert:
        Assert.Equal(expectedResult, actualResult.Value);
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
