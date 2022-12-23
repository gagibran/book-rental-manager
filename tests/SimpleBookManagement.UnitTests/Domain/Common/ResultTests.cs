using SimpleBookManagement.Domain.Exceptions;

namespace SimpleBookManagement.UnitTests.Domain.Common;

public sealed class ResultTests
{
    [Fact]
    public void Fail_WithEmptyErrorMessage_ThrowsException()
    {
        // Assert:
        Assert.Throws<UnsuccessfulResultMustHaveErrorMessageException>(
            () => Result<int>.Fail(" ")
        );
    }

    [Fact]
    public void Fail_WithErrorMessage_ThrowsException()
    {
        // Arrange:
        var errorMessage = "This is an error message.";

        // Act:
        var result = Result<int>.Fail(errorMessage);

        // Assert:
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void Success_WithValidValue_ReturnsValue()
    {
        // Arrange:
        var validValue = 1;

        // Act:
        var result = Result<int>.Success(validValue);

        // Assert:
        Assert.Equal(validValue, result.Value);
    }
}