namespace BookRentalManager.UnitTests.Application.SortParametersMappers;

public sealed class BookSortParametersMapperTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("NotExistingProperty,  , AnotherNotExistingOne", "")]
    public void Map_WithInputData_ReturnsExpectedOutputData(string propertyNamesSeparatedByComma, string expectedResult)
    {
        // Arrange:
        var bookSortParametersMapper = new BookSortParametersMapper();
        var bookSortParameters = new BookSortParameters(propertyNamesSeparatedByComma);

        // Act:
        string actualResult = bookSortParametersMapper.Map(bookSortParameters);

        // Assert:
        Assert.Equal(expectedResult, actualResult);
    }
}
