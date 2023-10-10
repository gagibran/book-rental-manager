namespace BookRentalManager.UnitTests.Domain.ValueObjects;

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

    [Theory]
    [InlineData("", "  ")]
    [InlineData("2es-d@2dsd.asd ", " 2es-d@2dsd.asd     ")]
    public void Create_CompareTwoIdenticalEmails_ReturnsTrue(string left, string right)
    {
        // Arrange:
        Result<Email> emailResultLeft = Email.Create(left);
        Result<Email> emailResultRight = Email.Create(right);

        // Act:
        bool isEqual = emailResultLeft.Value! == emailResultRight.Value!;

        // Assert:
        Assert.True(isEqual);
    }

    [Theory]
    [InlineData("", "2es-d@2dsd.asd")]
    [InlineData("2es-d@2dsd.asd", "2es-d@2dsd.rty")]
    public void Create_CompareTwoDifferentEmails_ReturnsFalse(string left, string right)
    {
        // Arrange:
        Result<Email> emailResultLeft = Email.Create(left);
        Result<Email> emailResultRight = Email.Create(right);

        // Act:
        bool isNotEqual = emailResultLeft.Value! != emailResultRight.Value!;

        // Assert:
        Assert.True(isNotEqual);
    }
}
