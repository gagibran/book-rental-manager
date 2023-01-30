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

    private void AddErrorsToModelError(string errorTypes, string errorMessages)
    {
        string[] splitErrorTypes = errorTypes.Split('|');
        string[] splitErrorMessages = errorMessages.Split('|');
        for (int i = 0; i < splitErrorMessages.Length; i++)
        {
            ModelState.AddModelError(splitErrorTypes[i], splitErrorMessages[i]);
        }
    }

    protected virtual ActionResult CustomUnprocessableEntity(string errorTypes, string errorMessages)
    {
        AddErrorsToModelError(errorTypes, errorMessages);
        return ValidationProblem(modelStateDictionary: ModelState, statusCode: 422);
    }

    protected virtual ActionResult<TDto> CustomNotFound<TDto>(string errorTypes, string errorMessages)
    {
        AddErrorsToModelError(errorTypes, errorMessages);
        return ValidationProblem(modelStateDictionary: ModelState, statusCode: 404);
    }
}
