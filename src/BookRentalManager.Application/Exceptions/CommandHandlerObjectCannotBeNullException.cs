namespace BookRentalManager.Application.Exceptions;

public sealed class CommandHandlerObjectCannotBeNullException : Exception
{
    public CommandHandlerObjectCannotBeNullException() : base("The command handler object cannot be null.")
    {
    }
}
