using BookRentalManager.Application.Authors.Commands;
using BookRentalManager.Application.Authors.Queries;

namespace BookRentalManager.Api.Controllers.V1;

[ApiVersion("1.0")]
public sealed class AuthorController : ApiController
{
    public AuthorController(IDispatcher dispatcher, ILogger<AuthorController> authorControllerLogger)
        : base(dispatcher, authorControllerLogger)
    {
    }

    [HttpGet(Name = nameof(GetAuthorsByQueryParametersAsync))]
    [HttpHead]
    public async Task<ActionResult<PaginatedList<GetAuthorDto>>> GetAuthorsByQueryParametersAsync(
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var getAuthorsByQueryParametersQuery = new GetAuthorsByQueryParametersQuery(
            queryParameters.PageIndex,
            queryParameters.PageSize,
            queryParameters.SearchQuery,
            queryParameters.SortBy);
        Result<PaginatedList<GetAuthorDto>> getAllAuthorsResult = await _dispatcher.DispatchAsync<PaginatedList<GetAuthorDto>>(
                getAuthorsByQueryParametersQuery,
                cancellationToken);
        if (!getAllAuthorsResult.IsSuccess)
        {
            return HandleError(getAllAuthorsResult);
        }
        CreatePagingMetadata(
            nameof(GetAuthorsByQueryParametersAsync),
            queryParameters.SearchQuery,
            queryParameters.SortBy,
            getAllAuthorsResult.Value!);
        return Ok(getAllAuthorsResult.Value);
    }

    [HttpGet("{id}")]
    [HttpHead("{id}")]
    [ActionName(nameof(GetAuthorByIdAsync))]
    public async Task<ActionResult<GetAuthorDto>> GetAuthorByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<GetAuthorDto> getAuthorByIdResult = await _dispatcher.DispatchAsync<GetAuthorDto>(
            new GetAuthorByIdQuery(id),
            cancellationToken);
        if (!getAuthorByIdResult.IsSuccess)
        {
            return HandleError(getAuthorByIdResult);
        }
        return Ok(getAuthorByIdResult.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAuthorAsync(CreateAuthorCommand createAuthorCommand, CancellationToken cancellationToken)
    {
        Result<AuthorCreatedDto> createAuthorResult = await _dispatcher.DispatchAsync<AuthorCreatedDto>(
            createAuthorCommand,
            cancellationToken);
        if (!createAuthorResult.IsSuccess)
        {
            return HandleError(createAuthorResult);
        }
        return CreatedAtAction(nameof(GetAuthorByIdAsync), new { Id = createAuthorResult.Value!.Id }, createAuthorResult.Value);
    }

    [HttpPatch("{id}/addBooks")]
    public async Task<ActionResult> AddExistingBooksToAuthor(
        Guid id,
        JsonPatchDocument<PatchAuthorBooksDto> patchAuthorBooksDtoPatchDocument,
        CancellationToken cancellationToken)
    {
        var patchAuthorBooksCommand = new PatchAuthorBooksCommand(id, patchAuthorBooksDtoPatchDocument);
        Result patchAuthorBooksResult = await _dispatcher.DispatchAsync(patchAuthorBooksCommand, cancellationToken);
        if (!patchAuthorBooksResult.IsSuccess)
        {
            return HandleError(patchAuthorBooksResult);
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuthorByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result deleteAuthorByIdResult = await _dispatcher.DispatchAsync(new DeleteAuthorByIdCommand(id), cancellationToken);
        if (!deleteAuthorByIdResult.IsSuccess)
        {
            return HandleError(deleteAuthorByIdResult);
        }
        return NoContent();
    }

    [HttpOptions("{id}/addBooks")]
    public async Task<ActionResult> GetAuthorAddBooksOptionsAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<GetAuthorDto> getAuthorByIdResult = await _dispatcher.DispatchAsync<GetAuthorDto>(
            new GetAuthorByIdQuery(id),
            cancellationToken);
        if (!getAuthorByIdResult.IsSuccess)
        {
            return HandleError(getAuthorByIdResult);
        }
        Response.Headers.Add("Allow", "PATCH, OPTIONS");
        return Ok();
    }

    [HttpOptions]
    public ActionResult GetAuthorOptions()
    {
        Response.Headers.Add("Allow", "GET, HEAD, POST, DELETE, OPTIONS");
        return Ok();
    }

    protected override ActionResult HandleError(Result result)
    {
        _baseControllerLogger.LogError(result.ErrorMessage);
        switch (result.ErrorType)
        {
            case "authorId":
            case "bookIds":
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.NotFound);
            case "jsonPatch":
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.BadRequest);
            default:
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.UnprocessableEntity);
        }
    }
}
