using System.Text.Json;

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

    protected void CreatePagingMetadata<TItem>(
        string routeName,
        string searchQuery,
        string sortQuery,
        PaginatedList<TItem> paginatedList)
    {
        string? previousPageLink = null;
        string? nextPageLink = null;
        if (paginatedList.HasPreviousPage)
        {
            previousPageLink = Url.Link(routeName, new
            {
                PageIndex = paginatedList.PageIndex - 1,
                PageSize = paginatedList.PageSize,
                SearchQuery = searchQuery,
                SortQuery = sortQuery
            });
        }
        if (paginatedList.HasNextPage)
        {
            nextPageLink = Url.Link(routeName, new
            {
                PageIndex = paginatedList.PageIndex + 1,
                PageSize = paginatedList.PageSize,
                SearchQuery = searchQuery,
                SortQuery = sortQuery
            });
        }
        int totalAmountOfPages = paginatedList.TotalAmountOfPages == int.MinValue ? 0 : paginatedList.TotalAmountOfPages;
        string serializedMetadata = JsonSerializer.Serialize(
            new
            {
                TotalAmountOfItems = paginatedList.TotalAmountOfItems,
                PageIndex = paginatedList.PageIndex,
                PageSize = paginatedList.PageSize,
                TotalAmountOfPages = totalAmountOfPages,
                PreviousPageLink = previousPageLink,
                NextPageLink = nextPageLink
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        Response.Headers.Add("X-Pagination", serializedMetadata);
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
