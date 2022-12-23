using SimpleBookManagement.Domain.Exceptions;

namespace SimpleBookManagement.Domain.Common;

public class Result<TValue>
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    private Result(bool isSuccess, TValue? value, string? errorMessage = null)
    {
        if (!isSuccess && string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new UnsuccessfulResultMustHaveErrorMessageException();
        }
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(true, value);
    }

    public static Result<TValue> Fail(string errorMessage)
    {
        return new Result<TValue>(false, default(TValue), errorMessage);
    }
}
