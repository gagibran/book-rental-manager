namespace BookRentalManager.Domain.Exceptions;

public sealed class UnsuccessfulResultMustHaveErrorMessageException : Exception
{
    public UnsuccessfulResultMustHaveErrorMessageException()
        : base("An unsuccessful result must have an error message.")
    {
    }
}
