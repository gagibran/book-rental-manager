using BookRentalManager.Domain.Exceptions;

namespace BookRentalManager.UnitTests.Domain.Common;

public sealed class ResultTests
{
    [Fact]
    public void Fail_WithEmptyErrorMessage_ThrowsException()
    {
        // Assert:
        Assert.Throws<UnsuccessfulResultMustHaveErrorMessageException>(
            () => Result.Fail(" ")
        );
    }

    [Fact]
    public void Fail_WithErrorMessage_ReturnsErrorMessage()
    {
        // Arrange:
        var errorMessage = "This is an error message.";

        // Act:
        Result result = Result.Fail(errorMessage);

        // Assert:
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void Success_WithValidValue_ReturnsValue()
    {
        // Arrange:
        var validValue = 1;

        // Act:
        Result<int> result = Result.Success<int>(validValue);

        // Assert:
        Assert.Equal(validValue, result.Value);
    }

    [Fact]
    public void Success_WithoutAnyValue_ReturnsSuccessful()
    {
        // Act:
        Result result = Result.Success();

        // Assert:
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Fail_WithErrorMessage_ReturnsUnsuccessful()
    {
        // Act:
        Result result = Result.Fail("This is an error message.");

        // Assert:
        Assert.False(result.IsSuccess);
    }
}