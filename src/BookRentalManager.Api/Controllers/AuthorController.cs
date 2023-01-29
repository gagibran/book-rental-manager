using BookRentalManager.Application.Authors.Queries;

namespace BookRentalManager.Api.Controllers;

[Route("api/[controller]")]
public sealed class AuthorController : ApiController
{
    public AuthorController(IDispatcher dispatcher, ILogger<AuthorController> authorControllerLogger)
        : base(dispatcher, authorControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetAuthorDto>>> GetAuthorsByQueryParametersAsync(
        [FromQuery] GetAllQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var getAuthorsByQueryParametersQuery = new GetAuthorsByQueryParametersQuery(
            queryParameters.PageIndex,
            queryParameters.TotalItemsPerPage,
            queryParameters.Search);
        Result<IReadOnlyList<GetAuthorDto>> getAllAuthorsResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetAuthorDto>>(
                getAuthorsByQueryParametersQuery,
                cancellationToken);
        return Ok(getAllAuthorsResult.Value);
    }
}
