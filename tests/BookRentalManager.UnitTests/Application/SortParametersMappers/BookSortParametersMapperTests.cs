namespace BookRentalManager.UnitTests.Application.SortParametersMappers;

public sealed class BookSortParametersMapperTests
{
    [Theory]
    [InlineData("Isbn,IsAvailable,BookTitleDesc", "Isbn.IsbnValue,IsAvailable,BookTitleDesc")]
    [InlineData("IsbnDesc,BookTitle,IsAvailable,EditionDesc", "Isbn.IsbnValueDesc,BookTitle,IsAvailable,Edition.EditionNumberDesc")]
    public void Map_WithInputData_ReturnsExpectedOutputData(string propertyNamesSeparatedByComma, string expectedResult)
    {
        // Arrange:
        var bookSortParametersMapper = new BookSortParametersMapper();
        var bookSortParameters = new BookSortParameters(propertyNamesSeparatedByComma);

        // Act:
        Result<string> actualResult = bookSortParametersMapper.Map(bookSortParameters);

        // Assert:
        Assert.Equal(expectedResult, actualResult.Value);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("NotExistingProperty,BookTitle, AnotherNotExistingOne", "NotExistingProperty")]
    [InlineData("NotExistingProperty,  , AnotherNotExistingOne", "NotExistingProperty")]
    public void Map_WithInvalidInputData_ReturnsErrorMessage(string propertyNamesSeparatedByComma, string faultyProperty)
    {
        // Arrange:
        var expectedErrorMessage = $"The property '{faultyProperty}' does not exist for '{nameof(GetBookDto)}'.";
        var bookSortParametersMapper = new BookSortParametersMapper();
        var bookSortParameters = new BookSortParameters(propertyNamesSeparatedByComma);

        // Act:
        Result<string> actualResult = bookSortParametersMapper.Map(bookSortParameters);

        // Assert:
        Assert.Equal(expectedErrorMessage, actualResult.ErrorMessage);
    }
}
