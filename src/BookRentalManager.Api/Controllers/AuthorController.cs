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
        [FromQuery] GetAllQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var getAuthorsByQueryParametersQuery = new GetAuthorsByQueryParametersQuery(
            queryParameters.PageIndex,
            queryParameters.PageSize,
            queryParameters.SearchQuery);
        Result<PaginatedList<GetAuthorDto>> getAllAuthorsResult = await _dispatcher.DispatchAsync<PaginatedList<GetAuthorDto>>(
                getAuthorsByQueryParametersQuery,
                cancellationToken);
        CreatePagingMetadata(nameof(GetAuthorsByQueryParametersAsync), queryParameters.SearchQuery, getAllAuthorsResult.Value!);
        return Ok(getAllAuthorsResult.Value);
    }
}
