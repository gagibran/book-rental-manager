namespace BookRentalManager.Api.Common;

[ApiController]
[Route("api/[controller]")]
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
