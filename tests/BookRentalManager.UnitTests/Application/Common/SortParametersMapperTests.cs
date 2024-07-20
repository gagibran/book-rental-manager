namespace BookRentalManager.UnitTests.Application.Common;

public sealed class SortParametersMapperTests
{
    private readonly SortParametersMapper _sortParametersMapper;

    public SortParametersMapperTests()
    {
        _sortParametersMapper = new();
    }

    [Theory]
    [InlineData("FullName", "FullName.FirstName,FullName.LastName")]
    [InlineData("FullNameDesc", "FullName.FirstNameDesc,FullName.LastNameDesc")]
    [InlineData("FullNameDesc,CreatedAt", "FullName.FirstNameDesc,FullName.LastNameDesc,CreatedAt")]
    [InlineData("CreatedAtDesc,FullNameDesc", "CreatedAtDesc,FullName.FirstNameDesc,FullName.LastNameDesc")]
    public void MapAuthorSortParameters_WithValidAuthorInputData_ReturnsSuccessWithExpectedOutputData(
        string propertyNamesSeparatedByComma,
        string expectedResult)
    {
        // Act:
        Result<string> actualResult = _sortParametersMapper.MapAuthorSortParameters(propertyNamesSeparatedByComma);

        // Assert:
        Assert.Equal(expectedResult, actualResult.Value);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("NotExistingProperty,  , AnotherNotExistingOne", "NotExistingProperty")]
    [InlineData("NotExistingProperty,  ,FullName, AnotherNotExistingOne", "NotExistingProperty")]
    public void MapAuthorSortParameters_WithInvalidAuthorInputData_ReturnsErrorMessage(
        string propertyNamesSeparatedByComma,
        string faultyProperty)
    {
        // Arrange:
        var expectedErrorMessage = $"The property '{faultyProperty}' does not exist for '{nameof(GetAuthorDto)}'.";

        // Act:
        Result<string> actualResult = _sortParametersMapper.MapAuthorSortParameters(propertyNamesSeparatedByComma);

        // Assert:
        Assert.Equal(expectedErrorMessage, actualResult.ErrorMessage);
    }

    [Theory]
    [InlineData("Isbn,RentedAt,BookTitleDesc", "Isbn.IsbnValue,RentedAt,BookTitle.TitleDesc")]
    [InlineData("Isbn,DueDate,BookTitleDesc", "Isbn.IsbnValue,DueDate,BookTitle.TitleDesc")]
    [InlineData("IsbnDesc,BookTitle,RentedAt,EditionDesc", "Isbn.IsbnValueDesc,BookTitle.Title,RentedAt,Edition.EditionNumberDesc")]
    public void MapBookSortParameters_WithValidBookInputData_ReturnsExpectedOutputData(
        string propertyNamesSeparatedByComma,
        string expectedResult)
    {
        // Act:
        Result<string> actualResult = _sortParametersMapper.MapBookSortParameters(propertyNamesSeparatedByComma);

        // Assert:
        Assert.Equal(expectedResult, actualResult.Value);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("NotExistingProperty,BookTitle, AnotherNotExistingOne", "NotExistingProperty")]
    [InlineData("NotExistingProperty,  , AnotherNotExistingOne", "NotExistingProperty")]
    public void MapBookSortParameters_WithInvalidBookInputData_ReturnsErrorMessage(
        string propertyNamesSeparatedByComma,
        string faultyProperty)
    {
        // Arrange:
        var expectedErrorMessage = $"The property '{faultyProperty}' does not exist for '{nameof(GetBookDto)}'.";

        // Act:
        Result<string> actualResult = _sortParametersMapper.MapBookSortParameters(propertyNamesSeparatedByComma);

        // Assert:
        Assert.Equal(expectedErrorMessage, actualResult.ErrorMessage);
    }

    [Fact]
    public void MapCustomerSortParameters_WithValidCustomerInputData_ReturnsExpectedOutputData()
    {
        // Arrange:
        var expectedQuery = "Email.EmailAddressDesc,PhoneNumber.AreaCode,PhoneNumber.PrefixAndLineNumber,CustomerPointsDesc,CustomerStatus.CustomerTypeDesc,FullName.FirstName,FullName.LastName";
        var customerSortParameters = "EmailDesc,PhoneNumber,CustomerPointsDesc,CustomerStatusDesc,FullName";

        // Act:
        Result<string> actualResult = _sortParametersMapper.MapCustomerSortParameters(customerSortParameters);

        // Assert:
        Assert.Equal(expectedQuery, actualResult.Value);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("NotExistingProperty,  , AnotherNotExistingOne", "NotExistingProperty")]
    [InlineData("NotExistingProperty,PhoneNumberDesc, AnotherNotExistingOne   ,CustomerPoints", "NotExistingProperty")]
    public void MapCustomerSortParameters_WithInvalidCustomerInputData_ReturnsErrorMessage(
        string propertyNamesSeparatedByComma,
        string faultyProperty)
    {
        // Arrange:
        var expectedErrorMessage = $"The property '{faultyProperty}' does not exist for '{nameof(GetCustomerDto)}'.";

        // Act:
        Result<string> actualResult = _sortParametersMapper.MapCustomerSortParameters(propertyNamesSeparatedByComma);

        // Assert:
        Assert.Equal(expectedErrorMessage, actualResult.ErrorMessage);
    }
}
