using System.Text.Json;

namespace BookRentalManager.Api.Common;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
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
                SortBy = sortQuery
            });
        }
        if (paginatedList.HasNextPage)
        {
            nextPageLink = Url.Link(routeName, new
            {
                PageIndex = paginatedList.PageIndex + 1,
                PageSize = paginatedList.PageSize,
                SearchQuery = searchQuery,
                SortBy = sortQuery
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

    protected ActionResult CustomHttpErrorResponse(string errorTypes, string errorMessages, HttpStatusCode httpStatusCode)
    {
        AddErrorsToModelError(errorTypes, errorMessages);
        return ValidationProblem(modelStateDictionary: ModelState, statusCode: (int)httpStatusCode);
    }

    protected abstract ActionResult HandleError(Result result);
}
