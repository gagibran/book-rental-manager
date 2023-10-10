namespace BookRentalManager.Application.Exceptions;

public sealed class QueryHandlerObjectCannotBeNullException : Exception
{
    public QueryHandlerObjectCannotBeNullException() : base("The query handler object cannot be null.")
    {
    }
}
