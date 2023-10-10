namespace BookRentalManager.UnitTests.Application.SortParametersMappers;

public sealed class AuthorSortParametersMapperTests
{
    [Theory]
    [InlineData("FullName", "FullName.FirstName,FullName.LastName")]
    [InlineData("FullNameDesc", "FullName.FirstNameDesc,FullName.LastNameDesc")]
    [InlineData("FullNameDesc,CreatedAt", "FullName.FirstNameDesc,FullName.LastNameDesc,CreatedAt")]
    [InlineData("CreatedAtDesc,FullNameDesc", "CreatedAtDesc,FullName.FirstNameDesc,FullName.LastNameDesc")]
    public void Map_WithValidInputData_ReturnsSuccessWithExpectedOutputData(string propertyNamesSeparatedByComma, string expectedResult)
    {
        // Arrange:
        var authorSortParametersMapper = new AuthorSortParametersMapper();
        var authorSortParameters = new AuthorSortParameters(propertyNamesSeparatedByComma);

        // Act:
        Result<string> actualResult = authorSortParametersMapper.Map(authorSortParameters);

        // Assert:
        Assert.Equal(expectedResult, actualResult.Value);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("NotExistingProperty,  , AnotherNotExistingOne", "NotExistingProperty")]
    [InlineData("NotExistingProperty,  ,FullName, AnotherNotExistingOne", "NotExistingProperty")]
    public void Map_WithInvalidInputData_ReturnsErrorMessage(string propertyNamesSeparatedByComma, string faultyProperty)
    {
        // Arrange:
        var expectedErrorMessage = $"The property '{faultyProperty}' does not exist for '{nameof(GetAuthorDto)}'.";
        var authorSortParametersMapper = new AuthorSortParametersMapper();
        var authorSortParameters = new AuthorSortParameters(propertyNamesSeparatedByComma);

        // Act:
        Result<string> actualResult = authorSortParametersMapper.Map(authorSortParameters);

        // Assert:
        Assert.Equal(expectedErrorMessage, actualResult.ErrorMessage);
    }
}
