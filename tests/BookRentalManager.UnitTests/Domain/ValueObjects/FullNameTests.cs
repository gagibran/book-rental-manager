namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class FullNameTests
{
    [Theory]
    [InlineData(" ", "Doe", "firstName", "First name cannot be empty.")]
    [InlineData("John", "", "lastName", "Last name cannot be empty.")]
    public void Create_WithInvalidFullName_ReturnsErrorMessage(
        string firstName,
        string lastName,
        string expectedErrorType,
        string expectedErrorMessage)
    {
        // Act:
        Result<FullName> fullNameResult = FullName.Create(firstName, lastName);

        // Assert:
        Assert.Equal(expectedErrorType, fullNameResult.ErrorType);
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
