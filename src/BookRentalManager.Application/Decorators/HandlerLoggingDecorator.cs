using System.Text.Json;

namespace BookRentalManager.Application.Decorators;

internal sealed class HandlerLoggingDecorator<TRequest>(
    IRequestHandler<TRequest> requestHandler,
    ILogger<IRequestHandler<TRequest>> logger)
    : IRequestHandler<TRequest> where TRequest : IRequest
{
    public async Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogHandlerExecutionStart(typeof(TRequest), JsonSerializer.Serialize(request));
        Result handleAsyncResult = await requestHandler.HandleAsync(request, cancellationToken);
        if (!handleAsyncResult.IsSuccess)
        {
            logger.LogHandlerThrewError(typeof(TRequest), JsonSerializer.Serialize(request), handleAsyncResult.ErrorMessage);
        }
        logger.LogHandlerExecutionFinish(typeof(TRequest), JsonSerializer.Serialize(request));
        return handleAsyncResult;
    }
}

internal sealed class HandlerLoggingDecorator<TRequest, TResult>(
    IRequestHandler<TRequest, TResult> requestHandler,
    ILogger<IRequestHandler<TRequest, TResult>> logger) : IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    public async Task<Result<TResult>> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogHandlerExecutionStart(typeof(TRequest), JsonSerializer.Serialize(request));
        Result<TResult> handleAsyncResult = await requestHandler.HandleAsync(request, cancellationToken);
        if (!handleAsyncResult.IsSuccess)
        {
            logger.LogHandlerThrewError(typeof(TRequest), JsonSerializer.Serialize(request), handleAsyncResult.ErrorMessage);
        }
        else
        {
            logger.LogHandlerReturnedResponse(
                typeof(TRequest),
                JsonSerializer.Serialize(request),
                JsonSerializer.Serialize(handleAsyncResult.Value));
        }
        logger.LogHandlerExecutionFinish(typeof(TRequest), JsonSerializer.Serialize(request));
        return handleAsyncResult;
    }
}
