namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class VolumeTests
{
    [Fact]
    public void Create_WithInvalidVolumeNumber_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "The volume number can't be smaller than 1.";

        // Act:
        var volumeResult = Volume.Create(0);

        // Assert:
        Assert.Equal(expectedErrorMessage, volumeResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithInvalidVolumeNumber_ReturnsSuccess()
    {
        // Act:
        Result<Volume> volumeResult = Volume.Create(2);

        // Assert:
        Assert.True(volumeResult.IsSuccess);
    }
}
