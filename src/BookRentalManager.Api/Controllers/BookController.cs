using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.Queries;

namespace BookRentalManager.Api.Controllers;

[Route("api/author/{authorId}/[controller]")]
public sealed class BookController : ApiController
{
    public BookController(IDispatcher dispatcher, ILogger<BookController> authorControllerLogger)
        : base(dispatcher, authorControllerLogger)
    {
    }

    [HttpGet("/api/[controller]", Name = nameof(GetBooksByQueryParametersAsync))]
    [HttpHead("/api/[controller]")]
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

    [HttpGet(Name = nameof(GetBooksByQueryParametersFromAuthorAsync))]
    [HttpHead]
    public async Task<ActionResult<PaginatedList<GetBookDto>>> GetBooksByQueryParametersFromAuthorAsync(
        Guid authorId,
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var getBooksBySearchParameterFromAuthor = new GetBooksByQueryParametersFromAuthorQuery(
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
            nameof(GetBooksByQueryParametersFromAuthorAsync),
            queryParameters.SearchQuery,
            queryParameters.SortBy,
            getAllBooksResult.Value!);
        return Ok(getAllBooksResult.Value);
    }

    [HttpGet("{id}")]
    [HttpHead("{id}")]
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
            return HandleError(getBookByIdResult);
        }
        return Ok(getBookByIdResult.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBookForAuthorAsync(
        Guid authorId,
        CreateBookForAuthorDto createBookForAuthorDto,
        CancellationToken cancellationToken)
    {
        var createBookForAuthorCommand = new CreateBookForAuthorCommand(
            authorId,
            createBookForAuthorDto.BookTitle,
            createBookForAuthorDto.Edition,
            createBookForAuthorDto.Isbn);
        Result<BookCreatedForAuthorDto> createBookResult = await _dispatcher.DispatchAsync<BookCreatedForAuthorDto>(createBookForAuthorCommand, cancellationToken);
        if (!createBookResult.IsSuccess)
        {
            return HandleError(createBookResult);
        }
        return CreatedAtAction(
            nameof(GetBookByIdFromAuthorAsync),
            new { authorId, id = createBookResult.Value!.Id },
            createBookResult.Value);
    }

    protected override ActionResult HandleError(Result result)
    {
        _baseControllerLogger.LogError(result.ErrorMessage);
        switch (result.ErrorType)
        {
            case "bookId":
            case "authorId":
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.NotFound);
            case "jsonPatch":
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.BadRequest);
            default:
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.UnprocessableEntity);
        }
    }
}
