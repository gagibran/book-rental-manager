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
        int totalItemsPerPage = 50
    )
    {
        Result<IReadOnlyList<GetBookAuthorDto>> getAllBookAuthorsResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetBookAuthorDto>>(
            new GetBookAuthorsQuery(pageIndex, totalItemsPerPage),
            default
        );
        if (!getAllBookAuthorsResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getAllBookAuthorsResult.ErrorMessage);
            return NotFound(getAllBookAuthorsResult.ErrorMessage);
        }
        return Ok(getAllBookAuthorsResult.Value);
    }
}
