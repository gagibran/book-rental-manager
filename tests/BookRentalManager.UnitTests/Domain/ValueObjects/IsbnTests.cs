namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class IsbnTests
{
    [Theory]
    [InlineData("123456789")]
    [InlineData("1345678-9 1")]
    [InlineData("1 2 34 56 78-X")]
    public void Create_WithInvalidIsbn_ReturnsErrorMessage(string isbnValue)
    {
        // Arrange:
        var expectedErrorMessage = "Invalid ISBN format.";

        // Act:
        var isbnResult = Isbn.Create(isbnValue);

        // Assert:
        Assert.Equal(expectedErrorMessage, isbnResult.ErrorMessage);
    }

    [Theory]
    [InlineData("978-1-86197-876-9")]
    [InlineData("978 1 86197 876 9")]
    [InlineData("0-19-852663-6")]
    [InlineData("1 86197 271-7")]
    public void Create_WithValidIsbn_ReturnsSuccess(string isbnValue)
    {
        // Act:
        Result<Isbn> isbnResult = Isbn.Create(isbnValue);

        // Assert:
        Assert.True(isbnResult.IsSuccess);
    }
}
