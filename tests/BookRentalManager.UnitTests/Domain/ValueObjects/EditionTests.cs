namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class EditionTests
{
    [Fact]
    public void Create_WithInvalidEditionNumber_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "The edition number can't be smaller than 1.";

        // Act:
        var editionResult = Edition.Create(0);

        // Assert:
        Assert.Equal(expectedErrorMessage, editionResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithInvalidEditionNumber_ReturnsSuccess()
    {
        // Act:
        Result<Edition> editionResult = Edition.Create(2);

        // Assert:
        Assert.True(editionResult.IsSuccess);
    }
}
