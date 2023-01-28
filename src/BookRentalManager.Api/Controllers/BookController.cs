using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.Queries;

namespace BookRentalManager.Api.Controllers;

[Route("api/bookauthor/{bookAuthorId}/[controller]")]
public sealed class BookController : ApiController
{
    public BookController(IDispatcher dispatcher, ILogger<BookController> bookAuthorControllerLogger)
        : base(dispatcher, bookAuthorControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetBookDto>>> GetBooksFromBookAuthorAsync(
        Guid bookAuthorId,
        CancellationToken cancellationToken,
        int pageIndex = 1,
        int totalItemsPerPage = 50,
        [FromQuery(Name = "search")] string searchParameter = "")
    {
        var getBooksBySearchParameterQuery = new GetBooksBySearchParameterQuery(
            bookAuthorId,
            pageIndex,
            totalItemsPerPage,
            searchParameter);
        Result<IReadOnlyList<GetBookDto>> getAllBooksResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetBookDto>>(
                getBooksBySearchParameterQuery,
                cancellationToken);
        if (!getAllBooksResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getAllBooksResult.ErrorMessage);
            return CustomNotFound<IReadOnlyList<GetBookDto>>(getAllBooksResult.ErrorMessage);
        }
        return Ok(getAllBooksResult.Value);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetBookByIdFromBookAuthorAsync))]
    public async Task<ActionResult<GetBookDto>> GetBookByIdFromBookAuthorAsync(
        Guid bookAuthorId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var getBookByIdQuery = new GetBookByIdQuery(bookAuthorId, id);
        Result<GetBookDto> getBookByIdResult = await _dispatcher.DispatchAsync<GetBookDto>(getBookByIdQuery, cancellationToken);
        if (!getBookByIdResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getBookByIdResult.ErrorMessage);
            return CustomNotFound<GetBookDto>(getBookByIdResult.ErrorMessage);
        }
        return Ok(getBookByIdResult.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBookForBookAuthorAsync(
        Guid bookAuthorId,
        CreateBookDto createBookDto,
        CancellationToken cancellationToken)
    {
        Result<BookCreatedDto> createBookResult = await _dispatcher.DispatchAsync<BookCreatedDto>(
            new CreateBookCommand(bookAuthorId, createBookDto),
            cancellationToken);
        if (!createBookResult.IsSuccess)
        {
            _baseControllerLogger.LogError(createBookResult.ErrorMessage);
            return CustomUnprocessableEntity(createBookResult.ErrorMessage);
        }
        return CreatedAtAction(
            nameof(GetBookByIdFromBookAuthorAsync),
            new { bookAuthorId, id = createBookResult.Value!.Id },
            createBookResult.Value);
    }
}
