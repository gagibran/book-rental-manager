namespace BookRentalManager.Api.Common;

[ApiController]
public class ApiController : ControllerBase
{
    protected readonly IDispatcher _dispatcher;
    protected readonly ILogger<ApiController> _baseControllerLogger;

    protected ApiController(
        IDispatcher dispatcher,
        ILogger<ApiController> baseControllerLogger)
    {
        _baseControllerLogger = baseControllerLogger;
        _dispatcher = dispatcher;
    }
}
