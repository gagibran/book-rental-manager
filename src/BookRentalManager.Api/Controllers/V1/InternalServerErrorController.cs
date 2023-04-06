using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace BookRentalManager.Api.Controllers.V1;

[ApiVersion("1.0")]
public sealed class InternalServerErrorController : ApiController
{
    private readonly ILogger<InternalServerErrorController> _logger;

    public InternalServerErrorController(ILogger<InternalServerErrorController> logger)
    {
        _logger = logger;
    }

    [Route("/internalServerError")]
    public ActionResult HandleInternalServerError()
    {
        Exception exception = HttpContext.Features.Get<IExceptionHandlerFeature>()!.Error;
        _logger.LogCritical(
                "{Timestamp} An exception of type '{ExceptionType}' was thrown. Error message: {ErrorMessage} Stack trace: {StackTrace}.",
                DateTime.UtcNow,
                exception.GetType(),
                exception.Message,
                exception.StackTrace);
        return Problem();
    }
}
