namespace BookRentalManager.UnitTests.Application.SortParametersMappers;

public sealed class AuthorSortParametersMapperTests
{
    [Theory]
    [InlineData("FullName", "FullName.CompleteName")]
    [InlineData("FullNameDesc", "FullName.CompleteNameDesc")]
    [InlineData("FullNameDesc,CreatedAt", "FullName.CompleteNameDesc,CreatedAt")]
    [InlineData("CreatedAtDesc,FullNameDesc", "CreatedAtDesc,FullName.CompleteNameDesc")]
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
