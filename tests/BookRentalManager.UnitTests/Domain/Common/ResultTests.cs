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

    [Fact]
    public void Combine_WithAtLeastTwoFailedResult_ReturnsErrorMessagesCombined()
    {
        // Arrange:
        Result failedResult1 = Result.Fail("This result has failed.");
        Result failedResult2 = Result.Fail("This result has also failed.");
        Result successResult = Result.Success();

        // Act:
        Result combinedResults = Result.Combine(failedResult1, failedResult2, successResult);

        // Assert:
        Assert.Equal("This result has failed. This result has also failed.", combinedResults.ErrorMessage);
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
