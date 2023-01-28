namespace BookRentalManager.Api.Common;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected readonly IDispatcher _dispatcher;
    protected readonly ILogger<ApiController> _baseControllerLogger;

    protected ApiController(IDispatcher dispatcher, ILogger<ApiController> baseControllerLogger)
    {
        _baseControllerLogger = baseControllerLogger;
        _dispatcher = dispatcher;
    }

    protected virtual ActionResult CustomUnprocessableEntity(string errorMessage)
    {
        ModelState.AddModelError("error", errorMessage);
        return ValidationProblem(modelStateDictionary: ModelState, statusCode: 422);
    }

    protected virtual ActionResult<TDto> CustomNotFound<TDto>(string errorMessage)
    {
        ModelState.AddModelError("error", errorMessage);
        return ValidationProblem(modelStateDictionary: ModelState, statusCode: 404);
    }
}
