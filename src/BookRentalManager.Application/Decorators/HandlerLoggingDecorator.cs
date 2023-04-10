using System.Text.Json;

namespace BookRentalManager.Application.Decorators;

internal sealed class HandlerLoggingDecorator<TRequest> : IRequestHandler<TRequest> where TRequest : IRequest
{
    private readonly IRequestHandler<TRequest> _requestHandler;
    private readonly ILogger<IRequestHandler<TRequest>> _logger;

    public HandlerLoggingDecorator(
        IRequestHandler<TRequest> requestHandler,
        ILogger<IRequestHandler<TRequest>> logger)
    {
        _requestHandler = requestHandler;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogIfLevelEnabled(
            LogLevel.Information,
            "Executing request handler '{RequestName}' with request value: {RequestValue}.",
            typeof(TRequest),
            JsonSerializer.Serialize(request));
        Result handleAsyncResult = await _requestHandler.HandleAsync(request, cancellationToken);
        if (!handleAsyncResult.IsSuccess)
        {
            _logger.LogIfLevelEnabled(
                LogLevel.Warning,
                "'{RequestName}' with request value: {RequestValue} threw the following error: {ErrorMessage}",
                typeof(TRequest),
                JsonSerializer.Serialize(request),
                handleAsyncResult.ErrorMessage);
        }
        _logger.LogIfLevelEnabled(
            LogLevel.Information,
            "Finished executing request handler '{RequestName}' with request value: {RequestValue}.",
            typeof(TRequest),
            JsonSerializer.Serialize(request));
        return handleAsyncResult;
    }
}

internal sealed class HandlerLoggingDecorator<TRequest, TResult> : IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    private readonly IRequestHandler<TRequest, TResult> _requestHandler;
    private readonly ILogger<IRequestHandler<TRequest, TResult>> _logger;

    public HandlerLoggingDecorator(
        IRequestHandler<TRequest, TResult> requestHandler,
        ILogger<IRequestHandler<TRequest, TResult>> logger)
    {
        _requestHandler = requestHandler;
        _logger = logger;
    }

    public async Task<Result<TResult>> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogIfLevelEnabled(
            LogLevel.Information,
            "Executing request handler '{RequestName}' with request value: {RequestValue}.",
            typeof(TRequest),
            JsonSerializer.Serialize(request));
        Result<TResult> handleAsyncResult = await _requestHandler.HandleAsync(request, cancellationToken);
        if (!handleAsyncResult.IsSuccess)
        {
            _logger.LogIfLevelEnabled(
                LogLevel.Warning,
                "'{RequestName}' with request value: {RequestValue} threw the following error: {ErrorMessage}",
                typeof(TRequest),
                JsonSerializer.Serialize(request),
                handleAsyncResult.ErrorMessage);
        }
        else
        {
            _logger.LogIfLevelEnabled(
                LogLevel.Information,
                "'{RequestName}' with request value: {RequestValue} returned the following response: {ResponseValue}.",
                DateTime.UtcNow,
                JsonSerializer.Serialize(handleAsyncResult.Value));
        }
        _logger.LogIfLevelEnabled(
            LogLevel.Information,
            "Finished executing request handler '{RequestName}' with request value: {RequestValue}.",
            typeof(TRequest),
            JsonSerializer.Serialize(request));
        return handleAsyncResult;
    }
}
