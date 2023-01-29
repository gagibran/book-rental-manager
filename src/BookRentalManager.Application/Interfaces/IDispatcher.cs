namespace BookRentalManager.Application.Interfaces;

public interface IDispatcher
{
    Task<Result> DispatchAsync(ICommand command, CancellationToken cancellationToken);
    Task<Result<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken);
    Task<Result<TResult>> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken);
}
