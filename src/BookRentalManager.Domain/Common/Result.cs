using BookRentalManager.Domain.Exceptions;

namespace BookRentalManager.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }

    protected Result(bool isSuccess, string errorMessage)
    {
        if (!isSuccess && string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new UnsuccessfulResultMustHaveErrorMessageException();
        }
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    public static Result Fail(string errorMessage)
    {
        return new Result(false, errorMessage);
    }

    public static Result<TValue> Success<TValue>(TValue value)
    {
        return new Result<TValue>(true, value, string.Empty);
    }

    public static Result<TValue> Fail<TValue>(string errorMessage)
    {
        return new Result<TValue>(false, default(TValue), errorMessage);
    }

    public static Result Combine(params Result[] results)
    {
        var finalErrorMessage = string.Empty;
        foreach (Result result in results)
        {
            if (!result.IsSuccess)
            {
                finalErrorMessage += result.ErrorMessage + " ";
            }
        }
        var isSuccess = string.IsNullOrWhiteSpace(finalErrorMessage) ? true : false;
        return new Result(isSuccess, finalErrorMessage.Trim());
    }
}

public sealed class Result<TValue> : Result
{
    public TValue? Value { get; }

    internal Result(bool isSuccess, TValue? value, string errorMessage)
        : base(isSuccess, errorMessage)
    {
        Value = value;
    }
}
