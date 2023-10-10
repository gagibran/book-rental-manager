namespace BookRentalManager.Application.Interfaces;

public interface IDispatcher
{
    Task<Result> DispatchAsync(IRequest request, CancellationToken cancellationToken);
    Task<Result<TResult>> DispatchAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken);
}
