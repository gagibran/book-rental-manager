using BookRentalManager.Application.Books.Queries;

namespace BookRentalManager.Api.Controllers;

public sealed class BookController : BaseController
{
    public BookController(IDispatcher dispatcher, ILogger<BookController> bookAuthorControllerLogger)
        : base(dispatcher, bookAuthorControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetBookDto>>> GetBooksAsync(
        CancellationToken cancellationToken,
        int pageIndex = 1,
        int totalItemsPerPage = 50,
        [FromQuery(Name = "search")] string searchParameter = "")
    {
        var getBooksBySearchParameterQuery = new GetBooksBySearchParameterQuery(
            pageIndex,
            totalItemsPerPage,
            searchParameter);
        Result<IReadOnlyList<GetBookDto>> getAllBooksResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetBookDto>>(
                getBooksBySearchParameterQuery,
                cancellationToken);
        return Ok(getAllBooksResult.Value);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetBookByIdAsync))]
    public async Task<ActionResult<GetBookDto>> GetBookByIdAsync(CancellationToken cancellationToken, Guid id)
    {
        var getBookByIdQuery = new GetBookByIdQuery(id);
        Result<GetBookDto> getBookByIdResult = await _dispatcher.DispatchAsync<GetBookDto>(getBookByIdQuery, cancellationToken);
        if (!getBookByIdResult.IsSuccess)
        {
            return NotFound(getBookByIdResult.ErrorMessage);
        }
        return Ok(getBookByIdResult.Value);
    }
}
