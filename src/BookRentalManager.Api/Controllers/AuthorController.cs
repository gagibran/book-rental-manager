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
        CancellationToken cancellationToken,
        int pageIndex = 1,
        int totalItemsPerPage = 50,
        [FromQuery(Name = "search")] string searchParameter = "")
    {
        var getAuthorsByQueryParametersQuery = new GetAuthorsByQueryParametersQuery(
            pageIndex,
            totalItemsPerPage,
            searchParameter);
        Result<IReadOnlyList<GetAuthorDto>> getAllAuthorsResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetAuthorDto>>(
                getAuthorsByQueryParametersQuery,
                cancellationToken);
        return Ok(getAllAuthorsResult.Value);
    }
}
