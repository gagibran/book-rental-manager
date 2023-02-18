namespace BookRentalManager.UnitTests.Application.SortParametersMappers;

public sealed class CustomerSortParametersMapperTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("NotExistingProperty,  , AnotherNotExistingOne", "")]
    [InlineData("NotExistingProperty,PhoneNumberDesc, AnotherNotExistingOne   ,CustomerPoints",
        "PhoneNumber.CompletePhoneNumberDesc,CustomerPoints")]
    [InlineData("EmailDesc,PhoneNumber,CustomerPointsDesc,CustomerStatusDesc,FullName",
        "Email.EmailAddressDesc,PhoneNumber.CompletePhoneNumber,CustomerPointsDesc,CustomerStatus.CustomerTypeDesc,FullName.CompleteName")]
    public void Map_WithInputData_ReturnsExpectedOutputData(string propertyNamesSeparatedByComma, string expectedResult)
    {
        // Arrange:
        var customerSortParametersMapper = new CustomerSortParametersMapper();
        var customerSortParameters = new CustomerSortParameters(propertyNamesSeparatedByComma);

        // Act:
        string actualResult = customerSortParametersMapper.Map(customerSortParameters);

        // Assert:
        Assert.Equal(expectedResult, actualResult);
    }
}
