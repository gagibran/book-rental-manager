using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.Queries;

namespace BookRentalManager.Api.Controllers;

[Route("api/[controller]")]
public sealed class BookController : ApiController
{
    public BookController(IDispatcher dispatcher, ILogger<BookController> authorControllerLogger)
        : base(dispatcher, authorControllerLogger)
    {
    }

    [HttpGet(Name = nameof(GetBooksByQueryParametersAsync))]
    [HttpHead]
    public async Task<ActionResult<PaginatedList<GetBookDto>>> GetBooksByQueryParametersAsync(
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var getBooksByQueryParametersQuery = new GetBooksByQueryParametersQuery(
            queryParameters.PageIndex,
            queryParameters.PageSize,
            queryParameters.SearchQuery,
            queryParameters.SortBy);
        Result<PaginatedList<GetBookDto>> getAllBooksResult = await _dispatcher.DispatchAsync<PaginatedList<GetBookDto>>(
                getBooksByQueryParametersQuery,
                cancellationToken);
        if (!getAllBooksResult.IsSuccess)
        {
            return HandleError(getAllBooksResult);
        }
        CreatePagingMetadata(
            nameof(GetBooksByQueryParametersAsync),
            queryParameters.SearchQuery,
            queryParameters.SortBy,
            getAllBooksResult.Value!);
        return Ok(getAllBooksResult.Value);
    }

    [HttpGet("excludingAuthor/{authorId}", Name = nameof(GetBooksByQueryParametersExcludingFromAuthorAsync))]
    [HttpHead("excludingAuthor/{authorId}")]
    public async Task<ActionResult<PaginatedList<GetBookDto>>> GetBooksByQueryParametersExcludingFromAuthorAsync(
        Guid authorId,
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var getBooksBySearchParameterFromAuthor = new GetBooksByQueryParametersExcludingFromAuthorQuery(
            authorId,
            queryParameters.PageIndex,
            queryParameters.PageSize,
            queryParameters.SearchQuery,
            queryParameters.SortBy);
        Result<PaginatedList<GetBookDto>> getAllBooksResult = await _dispatcher.DispatchAsync<PaginatedList<GetBookDto>>(
                getBooksBySearchParameterFromAuthor,
                cancellationToken);
        if (!getAllBooksResult.IsSuccess)
        {
            return HandleError(getAllBooksResult);
        }
        CreatePagingMetadata(
            nameof(GetBooksByQueryParametersExcludingFromAuthorAsync),
            queryParameters.SearchQuery,
            queryParameters.SortBy,
            getAllBooksResult.Value!);
        return Ok(getAllBooksResult.Value);
    }

    [HttpGet("{id}")]
    [HttpHead("{id}")]
    [ActionName(nameof(GetBookByIdAsync))]
    public async Task<ActionResult<GetBookDto>> GetBookByIdAsync(
        Guid authorId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var getBookByIdQuery = new GetBookByIdQuery(id);
        Result<GetBookDto> getBookByIdResult = await _dispatcher.DispatchAsync<GetBookDto>(getBookByIdQuery, cancellationToken);
        if (!getBookByIdResult.IsSuccess)
        {
            return HandleError(getBookByIdResult);
        }
        return Ok(getBookByIdResult.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBookAsync(
        CreateBookCommand createBookCommand,
        CancellationToken cancellationToken)
    {
        Result<BookCreatedDto> createBookResult = await _dispatcher.DispatchAsync<BookCreatedDto>(createBookCommand, cancellationToken);
        if (!createBookResult.IsSuccess)
        {
            return HandleError(createBookResult);
        }
        return CreatedAtAction(nameof(GetBookByIdAsync), new { Id = createBookResult.Value!.Id }, createBookResult.Value);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchBookTitleEditionAndIsbnByIdAsync(
        Guid id,
        JsonPatchDocument<PatchBookTitleEditionAndIsbnByIdDto> patchBookTitleEditionAndIsbnByIdDtoPatchDocument,
        CancellationToken cancellationToken)
    {
        var patchBookTitleEditionAndIsbnByIdCommand = new PatchBookTitleEditionAndIsbnByIdCommand(
            id,
            patchBookTitleEditionAndIsbnByIdDtoPatchDocument);
        Result patchBookTitleEditionAndIsbnByIdResult = await _dispatcher.DispatchAsync(
            patchBookTitleEditionAndIsbnByIdCommand,
            cancellationToken);
        if (!patchBookTitleEditionAndIsbnByIdResult.IsSuccess)
        {
            return HandleError(patchBookTitleEditionAndIsbnByIdResult);
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBookByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result deleteBookByIdResult = await _dispatcher.DispatchAsync(new DeleteBookByIdCommand(id), cancellationToken);
        if (!deleteBookByIdResult.IsSuccess)
        {
            return HandleError(deleteBookByIdResult);
        }
        return NoContent();
    }

    [HttpOptions]
    public ActionResult GetBookOptions()
    {
        Response.Headers.Add("Allow", "GET, HEAD, POST, PATCH, DELETE, OPTIONS");
        return Ok();
    }

    [HttpOptions("excludingAuthor")]
    public ActionResult GetExcludingAuthorOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS");
        return Ok();
    }

    protected override ActionResult HandleError(Result result)
    {
        _baseControllerLogger.LogError(result.ErrorMessage);
        switch (result.ErrorType)
        {
            case "bookId":
            case "authorId":
            case "authorIds":
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.NotFound);
            case "jsonPatch":
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.BadRequest);
            default:
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.UnprocessableEntity);
        }
    }
}
