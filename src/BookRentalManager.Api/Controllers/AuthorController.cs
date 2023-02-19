using BookRentalManager.Application.Authors.Queries;

namespace BookRentalManager.Api.Controllers;

[Route("api/[controller]")]
public sealed class AuthorController : ApiController
{
    public AuthorController(IDispatcher dispatcher, ILogger<AuthorController> authorControllerLogger)
        : base(dispatcher, authorControllerLogger)
    {
    }

    [HttpGet(Name = nameof(GetAuthorsByQueryParametersAsync))]
    public async Task<ActionResult<PaginatedList<GetAuthorDto>>> GetAuthorsByQueryParametersAsync(
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var getAuthorsByQueryParametersQuery = new GetAuthorsByQueryParametersQuery(
            queryParameters.PageIndex,
            queryParameters.PageSize,
            queryParameters.SearchQuery,
            queryParameters.SortBy);
        Result<PaginatedList<GetAuthorDto>> getAllAuthorsResult = await _dispatcher.DispatchAsync<PaginatedList<GetAuthorDto>>(
                getAuthorsByQueryParametersQuery,
                cancellationToken);
        if (!getAllAuthorsResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getAllAuthorsResult.ErrorMessage);
            return CustomHttpErrorResponse(getAllAuthorsResult.ErrorType, getAllAuthorsResult.ErrorMessage, HttpStatusCode.BadRequest);
        }
        CreatePagingMetadata(
            nameof(GetAuthorsByQueryParametersAsync),
            queryParameters.SearchQuery,
            queryParameters.SortBy,
            getAllAuthorsResult.Value!);
        return Ok(getAllAuthorsResult.Value);
    }
}
