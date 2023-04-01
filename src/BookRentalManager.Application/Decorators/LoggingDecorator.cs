using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BookRentalManager.Application.Decorators;

public sealed class LoggingDecorator<TRequest> : IRequestHandler<TRequest> where TRequest : IRequest
{
    private readonly IRequestHandler<TRequest> _requestHandler;
    private readonly ILogger<IRequestHandler<TRequest>> _logger;

    public LoggingDecorator(
        IRequestHandler<TRequest> requestHandler,
        ILogger<IRequestHandler<TRequest>> logger)
    {
        _requestHandler = requestHandler;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Executing request handler '{RequestName}' with request value: {RequestValue}.",
            typeof(TRequest),
            JsonSerializer.Serialize(request));
        Result handleAsyncResult = await _requestHandler.HandleAsync(request, cancellationToken);
        if (!handleAsyncResult.IsSuccess)
        {
            _logger.LogWarning(
                "An error ocurred while executing the request handler '{RequestName}' with request value: {RequestValue}. Error message: {ErrorMessage}",
                typeof(TRequest),
                JsonSerializer.Serialize(request),
                handleAsyncResult.ErrorMessage);
        }
        _logger.LogInformation(
            "Finished executing request handler '{RequestName}' with request value: {RequestValue}.",
            typeof(TRequest),
            JsonSerializer.Serialize(request));
        return handleAsyncResult;
    }
}

public sealed class LoggingDecorator<TRequest, TResult> : IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    private readonly IRequestHandler<TRequest, TResult> _requestHandler;
    private readonly ILogger<IRequestHandler<TRequest, TResult>> _logger;

    public LoggingDecorator(
        IRequestHandler<TRequest, TResult> requestHandler,
        ILogger<IRequestHandler<TRequest, TResult>> logger)
    {
        _requestHandler = requestHandler;
        _logger = logger;
    }

    public async Task<Result<TResult>> HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Executing request handler '{RequestName}' with request value: {RequestValue}.",
            typeof(TRequest),
            JsonSerializer.Serialize(request));
        Result<TResult> handleAsyncResult = await _requestHandler.HandleAsync(request, cancellationToken);
        if (!handleAsyncResult.IsSuccess)
        {
            _logger.LogWarning(
                "An error ocurred while executing the request handler '{RequestName}' with request value: {RequestValue}. Error message: {ErrorMessage}",
                typeof(TRequest),
                JsonSerializer.Serialize(request),
                handleAsyncResult.ErrorMessage);
        }
        else
        {
            _logger.LogInformation("Response value: {ResponseValue}.", JsonSerializer.Serialize(handleAsyncResult.Value));
        }
        _logger.LogInformation(
            "Finished executing request handler '{RequestName}' with request value: {RequestValue}.",
            typeof(TRequest),
            JsonSerializer.Serialize(request));
        return handleAsyncResult;
    }
}
