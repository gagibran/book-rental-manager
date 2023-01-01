namespace BookRentalManager.Application.Interfaces;

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> HandleAsync(
        IQuery<TResult> query,
        CancellationToken cancellationToken
    );
}
