using BookRentalManager.Application.BooksAuthors.Queries;

namespace BookRentalManager.Api.Controllers;

[Route("api/[controller]")]
public sealed class BookAuthorController : BaseController
{
    public BookAuthorController(IDispatcher dispatcher, ILogger<BookAuthorController> bookAuthorControllerLogger)
        : base(dispatcher, bookAuthorControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetBookAuthorDto>>> GetBookAuthorsAsync(
        CancellationToken cancellationToken,
        int pageIndex = 1,
        int totalItemsPerPage = 50,
        [FromQuery(Name = "search")] string searchParameter = "")
    {
        var getBookAuthorsBySearchParameterQuery = new GetBookAuthorsBySearchParameterQuery(
            pageIndex,
            totalItemsPerPage,
            searchParameter);
        Result<IReadOnlyList<GetBookAuthorDto>> getAllBookAuthorsResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetBookAuthorDto>>(
                getBookAuthorsBySearchParameterQuery,
                cancellationToken);
        return Ok(getAllBookAuthorsResult.Value);
    }
}
