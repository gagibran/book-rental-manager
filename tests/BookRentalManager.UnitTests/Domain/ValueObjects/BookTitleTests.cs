namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class BookTitleTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidTitle_ReturnsErrorMessage(string? bookTitle)
    {
        // Arrange:
        var expectedErrorMessage = "The title can't be empty.";

        // Act:
        Result<BookTitle> bookTitleResult = BookTitle.Create(bookTitle!);

        // Assert:
        Assert.Equal("bookTitle", bookTitleResult.ErrorType);
        Assert.Equal(expectedErrorMessage, bookTitleResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithValidTitle_ReturnsSuccess()
    {
        // Act:
        Result<BookTitle> bookTitleResult = BookTitle.Create("A Cool Book");

        // Assert:
        Assert.True(bookTitleResult.IsSuccess);
    }
}
