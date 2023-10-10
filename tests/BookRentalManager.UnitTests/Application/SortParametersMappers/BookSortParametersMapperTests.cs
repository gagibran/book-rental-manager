namespace BookRentalManager.UnitTests.Application.SortParametersMappers;

public sealed class BookSortParametersMapperTests
{
    [Theory]
    [InlineData("Isbn,RentedAt,BookTitleDesc", "Isbn.IsbnValue,RentedAt,BookTitleDesc")]
    [InlineData("Isbn,DueDate,BookTitleDesc", "Isbn.IsbnValue,DueDate,BookTitleDesc")]
    [InlineData("IsbnDesc,BookTitle,RentedAt,EditionDesc", "Isbn.IsbnValueDesc,BookTitle,RentedAt,Edition.EditionNumberDesc")]
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
