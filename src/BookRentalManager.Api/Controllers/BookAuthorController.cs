using BookRentalManager.Application.BookAuthorCqrs.Queries;

namespace BookRentalManager.Api.Controllers;

public sealed class BookAuthorController : BaseController
{
    public BookAuthorController(IDispatcher dispatcher, ILogger<BookAuthorController> bookAuthorControllerLogger)
        : base(dispatcher, bookAuthorControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetBookAuthorDto>>> GetBookAuthorsAsync(
        int pageIndex = 1,
        int totalItemsPerPage = 50,
        [FromQuery(Name = "search")] string searchParameter = "")
    {
        var getBookAuthorsWithSearchParamQuery = new GetBookAuthorsWithSearchParamQuery(
            pageIndex,
            totalItemsPerPage,
            searchParameter);
        Result<IReadOnlyList<GetBookAuthorDto>> getAllBookAuthorsResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetBookAuthorDto>>(
                getBookAuthorsWithSearchParamQuery,
                default);
        return Ok(getAllBookAuthorsResult.Value);
    }
}
