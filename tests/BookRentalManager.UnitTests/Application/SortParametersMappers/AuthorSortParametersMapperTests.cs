namespace BookRentalManager.UnitTests.Application.SortParametersMappers;

public sealed class AuthorSortParametersMapperTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("NotExistingProperty,  , AnotherNotExistingOne", "")]
    [InlineData("NotExistingProperty,  ,FullName, AnotherNotExistingOne", "FullName.CompleteName")]
    [InlineData("FullName", "FullName.CompleteName")]
    [InlineData("FullNameDesc", "FullName.CompleteNameDesc")]
    [InlineData("FullNameDesc,CreatedAt", "FullName.CompleteNameDesc,CreatedAt")]
    [InlineData("CreatedAtDesc,FullNameDesc", "CreatedAtDesc,FullName.CompleteNameDesc")]
    public void Map_WithInputData_ReturnsExpectedOutputData(string propertyNamesSeparatedByComma, string expectedResult)
    {
        // Arrange:
        var authorSortParametersMapper = new AuthorSortParametersMapper();
        var authorSortParameters = new AuthorSortParameters(propertyNamesSeparatedByComma);

        // Act:
        string actualResult = authorSortParametersMapper.Map(authorSortParameters);

        // Assert:
        Assert.Equal(expectedResult, actualResult);
    }
}
