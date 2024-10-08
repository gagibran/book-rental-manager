﻿using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace BookRentalManager.Api.ExceptionHandlers;

#pragma warning disable CS1591
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        string traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        logger.LogInternalServerError(exception.GetType(), exception.Message, traceId);
        await Results
            .Problem(
                title: "An unexpected error occurred. Please, contact support and give them the trace ID to further assist you.",
                extensions: new Dictionary<string, object?> { { "traceId", traceId } })
            .ExecuteAsync(httpContext);
        return true;
    }
}
#pragma warning restore CS1591
