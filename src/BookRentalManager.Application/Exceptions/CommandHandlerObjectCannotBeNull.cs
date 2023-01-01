namespace BookRentalManager.Application.Exceptions;

public sealed class CommandHandlerObjectCannotBeNull : Exception
{
    public CommandHandlerObjectCannotBeNull()
        : base("The command handler object cannot be null.")
    {
    }
}
