namespace BookRentalManager.Application.Interfaces;

public interface IRequestHandler<TRequest> where TRequest : IRequest
{
    Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    Task<Result<TResult>> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
