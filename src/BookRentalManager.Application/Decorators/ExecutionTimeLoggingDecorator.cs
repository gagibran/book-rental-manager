using System.Diagnostics;
using System.Text.Json;

namespace BookRentalManager.Application.Decorators;

internal sealed class ExecutionTimeLoggingDecorator<TRequest>(
    IRequestHandler<TRequest> requestHandler,
    ILogger<IRequestHandler<TRequest>> logger)
    : IRequestHandler<TRequest> where TRequest : IRequest
{
    private readonly IRequestHandler<TRequest> _requestHandler = requestHandler;
    private readonly ILogger<IRequestHandler<TRequest>> _logger = logger;

    public async Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Result handleAsyncResult = await _requestHandler.HandleAsync(request, cancellationToken);
        stopwatch.Stop();
        _logger.LogHandlerExecutionTime(typeof(TRequest), JsonSerializer.Serialize(request), stopwatch.ElapsedMilliseconds);
        return handleAsyncResult;
    }
}

internal sealed class ExecutionTimeLoggingDecorator<TRequest, TResult>(
    IRequestHandler<TRequest, TResult> requestHandler,
    ILogger<IRequestHandler<TRequest, TResult>> logger) : IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    private readonly IRequestHandler<TRequest, TResult> _requestHandler = requestHandler;
    private readonly ILogger<IRequestHandler<TRequest, TResult>> _logger = logger;

    public async Task<Result<TResult>> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Result<TResult> handleAsyncResult = await _requestHandler.HandleAsync(request, cancellationToken);
        stopwatch.Stop();
        _logger.LogHandlerExecutionTime(typeof(TRequest), JsonSerializer.Serialize(request), stopwatch.ElapsedMilliseconds);
        return handleAsyncResult;
    }
}
