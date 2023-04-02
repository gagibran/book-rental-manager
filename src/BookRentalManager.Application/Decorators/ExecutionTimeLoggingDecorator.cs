using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BookRentalManager.Application.Decorators;

public sealed class ExecutionTimeLoggingDecorator<TRequest> : IRequestHandler<TRequest> where TRequest : IRequest
{
    private readonly IRequestHandler<TRequest> _requestHandler;
    private readonly ILogger<IRequestHandler<TRequest>> _logger;

    public ExecutionTimeLoggingDecorator(
        IRequestHandler<TRequest> requestHandler,
        ILogger<IRequestHandler<TRequest>> logger)
    {
        _requestHandler = requestHandler;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Result handleAsyncResult = await _requestHandler.HandleAsync(request, cancellationToken);
        stopwatch.Stop();
        _logger.LogDebug(
            "{Timestamp} Execution of request handler '{RequestName}' with request value: {RequestValue} took {ExecutionTimeInMilliSeconds} ms.",
            DateTime.UtcNow,
            typeof(TRequest),
            JsonSerializer.Serialize(request),
            stopwatch.ElapsedMilliseconds);
        return handleAsyncResult;
    }
}

public sealed class ExecutionTimeLoggingDecorator<TRequest, TResult> : IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    private readonly IRequestHandler<TRequest, TResult> _requestHandler;
    private readonly ILogger<IRequestHandler<TRequest, TResult>> _logger;

    public ExecutionTimeLoggingDecorator(
        IRequestHandler<TRequest, TResult> requestHandler,
        ILogger<IRequestHandler<TRequest, TResult>> logger)
    {
        _requestHandler = requestHandler;
        _logger = logger;
    }

    public async Task<Result<TResult>> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Result<TResult> handleAsyncResult = await _requestHandler.HandleAsync(request, cancellationToken);
        stopwatch.Stop();
        _logger.LogDebug(
            "{Timestamp} Execution of request handler '{RequestName}' with request value: {RequestValue} took {ExecutionTimeInMilliSeconds} ms.",
            DateTime.UtcNow,
            typeof(TRequest),
            JsonSerializer.Serialize(request),
            stopwatch.ElapsedMilliseconds);
        return handleAsyncResult;
    }
}
