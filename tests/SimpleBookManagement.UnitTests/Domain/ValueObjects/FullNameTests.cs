namespace SimpleBookManagement.UnitTests.Domain.ValueObjects;

public sealed class FullNameTests
{
    [Fact]
    public void Create_WithEmptyFirstName_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "First name cannot be empty.";

        // Act:
        Result<FullName> fullNameResult = FullName.Create(" ", "Doe");

        // Assert:
        Assert.Equal(expectedErrorMessage, fullNameResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithEmptyLastName_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "Last name cannot be empty.";

        // Act:
        Result<FullName> fullNameResult = FullName.Create("John", "");

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
}
