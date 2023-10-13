namespace BookRentalManager.UnitTests.Domain.Common;

public sealed class ResultTests
{
    [Theory]
    [InlineData("errorType", " ")]
    [InlineData("", "This is an error message.")]
    [InlineData(" ", "")]
    public void Fail_WithEmptyErrorMessage_ThrowsException(string errorType, string errorMessage)
    {
        // Assert:
        Assert.Throws<UnsuccessfulResultMustHaveErrorTypeWithErrorMessageException>(() => Result.Fail(errorType, errorMessage));
    }

    [Fact]
    public void Fail_WithErrorMessage_ReturnsErrorMessage()
    {
        // Arrange:
        var errorType = "errorType";
        var errorMessage = "This is an error message.";

        // Act:
        Result result = Result.Fail(errorType, errorMessage);

        // Assert:
        Assert.Equal(errorType, result.ErrorType);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void Success_WithValidValue_ReturnsValue()
    {
        // Arrange:
        var validValue = 1;

        // Act:
        Result<int> result = Result.Success(validValue);

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
        Result result = Result.Fail("errorType", "This is an error message.");

        // Assert:
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Combine_WithAtLeastTwoFailedResult_ReturnsErrorMessagesCombined()
    {
        // Arrange:
        Result failedResult1 = Result.Fail("errorType", "This result has failed.");
        Result failedResult2 = Result.Fail("anotherErrorType", "This result has also failed.");
        Result successResult = Result.Success();

        // Act:
        Result combinedResults = Result.Combine(failedResult1, failedResult2, successResult);

        // Assert:
        Assert.Equal("errorType|anotherErrorType", combinedResults.ErrorType);
        Assert.Equal("This result has failed.|This result has also failed.", combinedResults.ErrorMessage);
    }

    [Fact]
    public void Combine_WithoutFailedResults_ReturnsNullErrorMessage()
    {
        // Arrange:
        Result successResult1 = Result.Success();
        Result successResult2 = Result.Success();
        Result successResult3 = Result.Success();

        // Act:
        Result combinedResults = Result.Combine(successResult1, successResult2, successResult3);

        // Assert:
        Assert.Empty(combinedResults.ErrorMessage);
    }
}
