namespace BookRentalManager.UnitTests.Application.SortParametersMappers;

public sealed class BookSortParametersMapperTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("NotExistingProperty,  , AnotherNotExistingOne", "")]
    [InlineData("NotExistingProperty,BookTitle, AnotherNotExistingOne", "BookTitle")]
    [InlineData("Isbn,IsAvailable,BookTitleDesc", "Isbn.IsbnValue,IsAvailable,BookTitleDesc")]
    [InlineData("IsbnDesc,BookTitle,IsAvailable,EditionDesc", "Isbn.IsbnValueDesc,BookTitle,IsAvailable,Edition.EditionNumberDesc")]
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
