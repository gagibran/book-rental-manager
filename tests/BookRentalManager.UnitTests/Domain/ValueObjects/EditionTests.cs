namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class EditionTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void Create_WithInvalidEditionNumber_ReturnsErrorMessage(int editionNumber)
    {
        // Arrange:
        var expectedErrorMessage = "The edition number can't be smaller than 1.";

        // Act:
        Result<Edition> editionResult = Edition.Create(editionNumber);

        // Assert:
        Assert.Equal("editionNumber", editionResult.ErrorType);
        Assert.Equal(expectedErrorMessage, editionResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithValidEditionNumber_ReturnsSuccess()
    {
        // Act:
        Result<Edition> editionResult = Edition.Create(2);

        // Assert:
        Assert.True(editionResult.IsSuccess);
    }
}
