using System.Diagnostics;
using System.Text.Json;

namespace BookRentalManager.Application.Decorators;

internal sealed class ExecutionTimeLoggingDecorator<TRequest>(
    IRequestHandler<TRequest> requestHandler,
    ILogger<IRequestHandler<TRequest>> logger)
    : IRequestHandler<TRequest> where TRequest : IRequest
{
    public async Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Result handleAsyncResult = await requestHandler.HandleAsync(request, cancellationToken);
        stopwatch.Stop();
        logger.LogHandlerExecutionTime(typeof(TRequest), JsonSerializer.Serialize(request), stopwatch.ElapsedMilliseconds);
        return handleAsyncResult;
    }
}

internal sealed class ExecutionTimeLoggingDecorator<TRequest, TResult>(
    IRequestHandler<TRequest, TResult> requestHandler,
    ILogger<IRequestHandler<TRequest, TResult>> logger) : IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    public async Task<Result<TResult>> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Result<TResult> handleAsyncResult = await requestHandler.HandleAsync(request, cancellationToken);
        stopwatch.Stop();
        logger.LogHandlerExecutionTime(typeof(TRequest), JsonSerializer.Serialize(request), stopwatch.ElapsedMilliseconds);
        return handleAsyncResult;
    }
}
