using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.Queries;

namespace BookRentalManager.Api.Controllers;

[Route("api/bookauthor/{authorId}/[controller]")]
public sealed class BookController : ApiController
{
    public BookController(IDispatcher dispatcher, ILogger<BookController> authorControllerLogger)
        : base(dispatcher, authorControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetBookDto>>> GetBooksByQueryParametersFromAuthorAsync(
        Guid authorId,
        CancellationToken cancellationToken,
        int pageIndex = 1,
        int totalItemsPerPage = 50,
        [FromQuery(Name = "search")] string searchParameter = "")
    {
        var getBooksBySearchParameterFromAuthor = new GetBooksByQueryParametersFromAuthorQuery(
            authorId,
            pageIndex,
            totalItemsPerPage,
            searchParameter);
        Result<IReadOnlyList<GetBookDto>> getAllBooksResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetBookDto>>(
                getBooksBySearchParameterFromAuthor,
                cancellationToken);
        if (!getAllBooksResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getAllBooksResult.ErrorMessage);
            return CustomNotFound<IReadOnlyList<GetBookDto>>(getAllBooksResult.ErrorMessage);
        }
        return Ok(getAllBooksResult.Value);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetBookByIdFromAuthorAsync))]
    public async Task<ActionResult<GetBookDto>> GetBookByIdFromAuthorAsync(
        Guid authorId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var getBookByIdFromAuthorQuery = new GetBookByIdFromAuthorQuery(authorId, id);
        Result<GetBookDto> getBookByIdResult = await _dispatcher.DispatchAsync<GetBookDto>(getBookByIdFromAuthorQuery, cancellationToken);
        if (!getBookByIdResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getBookByIdResult.ErrorMessage);
            return CustomNotFound<GetBookDto>(getBookByIdResult.ErrorMessage);
        }
        return Ok(getBookByIdResult.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBookForAuthorAsync(
        Guid authorId,
        CreateBookForAuthorCommand createBookForAuthorCommand,
        CancellationToken cancellationToken)
    {
        Result<BookCreatedDto> createBookResult = await _dispatcher.DispatchAsync<BookCreatedDto>(createBookForAuthorCommand, cancellationToken);
        if (!createBookResult.IsSuccess)
        {
            _baseControllerLogger.LogError(createBookResult.ErrorMessage);
            return CustomUnprocessableEntity(createBookResult.ErrorMessage);
        }
        return CreatedAtAction(
            nameof(GetBookByIdFromAuthorAsync),
            new { authorId, id = createBookResult.Value!.Id },
            createBookResult.Value);
    }
}
