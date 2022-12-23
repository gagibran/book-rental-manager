namespace SimpleBookManagement.UnitTests.Domain.ValueObjects;

public sealed class EmailTests
{
    [Fact]
    public void Create_WithInvalidEmail_ReturnsErrorMessage()
    {
        // Arrange:
        var expectedErrorMessage = "Email address is not in a valid format.";

        // Act:
        Result<Email> emailResult = Email.Create(".2es-d@2dsd.asd");

        // Assert:
        Assert.Equal(expectedErrorMessage, emailResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithValidEmail_ReturnsSuccess()
    {
        // Act:
        Result<Email> emailResult = Email.Create("2es-d@2dsd.asd");

        // Assert:
        Assert.True(emailResult.IsSuccess);
    }
}
