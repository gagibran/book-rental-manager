using BookRentalManager.Application.BookCqrs.Queries;
using BookRentalManager.Application.Dtos.BookDtos;

namespace BookRentalManager.Api.Controllers;

public sealed class BookController : BaseController
{
    public BookController(IDispatcher dispatcher, ILogger<BookController> bookAuthorControllerLogger)
        : base(dispatcher, bookAuthorControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetBookDto>>> GetBooksAsync(
        int pageIndex = 1,
        int totalItemsPerPage = 50,
        [FromQuery(Name = "search")] string searchParameter = "")
    {
        var getBooksWithBooksAndSearchParamQuery = new GetBooksWithSearchParamQuery(
            pageIndex,
            totalItemsPerPage,
            searchParameter);
        Result<IReadOnlyList<GetBookDto>> getAllBooksResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetBookDto>>(
                getBooksWithBooksAndSearchParamQuery,
                default);
        return Ok(getAllBooksResult.Value);
    }
}
