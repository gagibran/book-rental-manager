namespace BookRentalManager.Application.Exceptions;

public sealed class HandlerObjectCannotBeNullException : Exception
{
    public HandlerObjectCannotBeNullException() : base("The handler object cannot be null.")
    {
    }
}
