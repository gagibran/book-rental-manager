using BookRentalManager.Application.Exceptions;

namespace BookRentalManager.Application.Common;

public sealed class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result> DispatchAsync(IRequest request, CancellationToken cancellationToken)
    {
        Type requestHandlerType = typeof(IRequestHandler<>);
        Type requestType = request.GetType();
        Type requestHandlerGenericType = requestHandlerType.MakeGenericType(requestType);
        dynamic? requestHandler = _serviceProvider.GetService(requestHandlerGenericType);
        if (requestHandler is null)
        {
            throw new CommandHandlerObjectCannotBeNullException();
        }
        return await requestHandler.HandleAsync((dynamic)request, cancellationToken);
    }

    public async Task<Result<TResult>> DispatchAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken)
    {
        Type requestHandlerType = typeof(IRequestHandler<,>);
        Type[] requestHandlerArgumentTypes =
        {
            request.GetType(),
            typeof(TResult)
        };
        Type requestHandlerGenericType = requestHandlerType.MakeGenericType(requestHandlerArgumentTypes);
        dynamic? requestHandler = _serviceProvider.GetService(requestHandlerGenericType);
        if (requestHandler is null)
        {
            throw new CommandHandlerObjectCannotBeNullException();
        }
        return await requestHandler.HandleAsync((dynamic)request, cancellationToken);
    }
}
