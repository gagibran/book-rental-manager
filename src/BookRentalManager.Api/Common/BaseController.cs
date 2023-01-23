namespace BookRentalManager.Api.Common;

[ApiController]
public class BaseController : ControllerBase
{
    protected readonly IDispatcher _dispatcher;
    protected readonly ILogger<BaseController> _baseControllerLogger;

    protected BaseController(
        IDispatcher dispatcher,
        ILogger<BaseController> baseControllerLogger)
    {
        _baseControllerLogger = baseControllerLogger;
        _dispatcher = dispatcher;
    }
}
