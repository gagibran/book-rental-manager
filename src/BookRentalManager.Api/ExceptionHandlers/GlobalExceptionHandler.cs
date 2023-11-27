using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace BookRentalManager.Api.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        string traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        _logger.LogInternalServerError(exception.GetType(), exception.Message, traceId);
        await Results
            .Problem(
                title: "An unexpected error occurred. Please, contact support and give them the trace ID to further assist you.",
                extensions: new Dictionary<string, object?> { { "traceId", traceId } })
            .ExecuteAsync(httpContext);
        return true;
    }
}
