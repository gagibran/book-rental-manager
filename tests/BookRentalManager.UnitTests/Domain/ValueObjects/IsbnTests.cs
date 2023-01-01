namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class IsbnTests
{
    [Theory]
    [InlineData(999_999_999)]
    [InlineData(10_000_000_000)]
    public void Create_WithInvalidIsbn_ReturnsErrorMessage(long isbnNumber)
    {
        // Arrange:
        var expectedErrorMessage = "Invalid ISBN format.";

        // Act:
        var isbnResult = Isbn.Create(isbnNumber);

        // Assert:
        Assert.Equal(expectedErrorMessage, isbnResult.ErrorMessage);
    }

    [Theory]
    [InlineData(1_000_000_000)]
    [InlineData(9_999_999_999)]
    [InlineData(2_345_976_098)]
    public void Create_WithValidIsbn_ReturnsSuccess(long isbnNumber)
    {
        // Act:
        Result<Isbn> isbnResult = Isbn.Create(isbnNumber);

        // Assert:
        Assert.True(isbnResult.IsSuccess);
    }
}
