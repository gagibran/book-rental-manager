namespace BookRentalManager.Application.Exceptions;

public sealed class QueryHandlerObjectCannotBeNull : Exception
{
    public QueryHandlerObjectCannotBeNull()
        : base("The query handler object cannot be null.")
    {
    }
}
