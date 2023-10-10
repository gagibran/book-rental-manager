namespace BookRentalManager.Domain.Exceptions;

public sealed class QueryableIsNotAsyncEnumerableException : Exception
{
    public QueryableIsNotAsyncEnumerableException()
        : base("IQueryable is not of type IAsyncEnumerable.")
    {
    }
}
