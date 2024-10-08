using BookRentalManager.Application.Enums;

namespace BookRentalManager.Application.Extensions;

public static partial class ILoggerExtensions
{
    [LoggerMessage(
        (int)EventIds.LogHandlerExecutionTime,
        LogLevel.Debug,
        "Execution of request handler '{RequestHandlerType}' with request value: {RequestValue} took {ExecutionTimeInMilliSeconds} ms.")]
    public static partial void LogHandlerExecutionTime(
        this ILogger logger,
        Type requestHandlerType,
        string requestValue,
        long executionTimeInMilliSeconds);

    [LoggerMessage(
        (int)EventIds.LogHandlerExecutionStart,
        LogLevel.Information,
        "Executing request handler '{RequestHandlerType}' with request value: {RequestValue}.")]
    public static partial void LogHandlerExecutionStart(this ILogger logger, Type requestHandlerType, string requestValue);

    [LoggerMessage(
        (int)EventIds.LogHandlerThrewError,
        LogLevel.Warning,
        "The '{RequestHandlerType}' handler with request value: {RequestValue} threw the following error: {ErrorMessage}",
        SkipEnabledCheck = true)]
    public static partial void LogHandlerThrewError(
        this ILogger logger,
        Type requestHandlerType,
        string requestValue,
        string errorMessage);

    [LoggerMessage(
        (int)EventIds.LogHandlerReturnedResponse,
        LogLevel.Information,
        "'The '{RequestHandlerType}' handler with request value: {RequestValue} returned the following response: {ResponseValue}.")]
    public static partial void LogHandlerReturnedResponse(
        this ILogger logger,
        Type requestHandlerType,
        string requestValue,
        object responseValue);

    [LoggerMessage(
        (int)EventIds.LogHandlerExecutionFinish,
        LogLevel.Information,
        "Executing request handler '{RequestHandlerType}' with request value: {RequestValue}.")]
    public static partial void LogHandlerExecutionFinish(this ILogger logger, Type requestHandlerType, string requestValue);

    [LoggerMessage(
        (int)EventIds.LogInternalServerError,
        LogLevel.Error,
        "An exception of type '{ExceptionType}' was thrown. Error message: {ErrorMessage} Trace ID: {TraceId}.",
        SkipEnabledCheck = true)]
    public static partial void LogInternalServerError(
        this ILogger logger,
        Type exceptionType,
        string errorMessage,
        string? traceId);
}
