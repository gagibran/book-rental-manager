namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class FullNameTests
{
    [Theory]
    [InlineData(" ", "Doe", "First name cannot be empty.")]
    [InlineData("John", "", "Last name cannot be empty.")]
    public void Create_WithInvalidFullName_ReturnsErrorMessage(
        string firstName,
        string lastName,
        string expectedErrorMessage)
    {
        // Act:
        Result<FullName> fullNameResult = FullName.Create(firstName, lastName);

        // Assert:
        Assert.Equal(expectedErrorMessage, fullNameResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithValidFullName_ReturnsSuccess()
    {
        // Act:
        Result<FullName> fullNameResult = FullName.Create("John", "Doe");

        // Assert:
        Assert.True(fullNameResult.IsSuccess);
    }

    [Theory]
    [InlineData("John", "Doe")]
    [InlineData("John ", "  Doe ")]
    public void Create_WithValidFullName_ReturnsCorrectlyFormattedFullName(
        string firstName,
        string lastName)
    {
        // Arrange:
        var expectedFullName = "John Doe";

        // Act:
        Result<FullName> fullNameResult = FullName.Create(firstName, lastName);

        // Assert:
        Assert.Equal(expectedFullName, fullNameResult.Value!.ToString());
    }
}
