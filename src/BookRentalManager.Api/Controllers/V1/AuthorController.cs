using BookRentalManager.Application.Authors.Commands;
using BookRentalManager.Application.Authors.Queries;

namespace BookRentalManager.Api.Controllers.V1;

[ApiVersion("1.0")]
public sealed class AuthorController : ApiController
{
    private readonly IDispatcher _dispatcher;
    private readonly List<AllowedRestMethodsDto> _allowedRestMethodDtos;

    public AuthorController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _allowedRestMethodDtos = new List<AllowedRestMethodsDto>
        {
            new AllowedRestMethodsDto(nameof(GetAuthorByIdAsync), "GET", "self"),
            new AllowedRestMethodsDto(nameof(AddExistingBooksToAuthor), "PATCH", "add_existing_books_to_author"),
            new AllowedRestMethodsDto(nameof(DeleteAuthorByIdAsync), "DELETE", "delete_author")
        };
    }

    [HttpGet(Name = nameof(GetAuthorsByQueryParametersAsync))]
    [HttpHead]
    public async Task<ActionResult<PaginatedList<GetAuthorDto>>> GetAuthorsByQueryParametersAsync(
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        [FromHeader(Name = "Accept")] string? mediaType,
        CancellationToken cancellationToken)
    {
        if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? mediaTypeHeaderValue))
        {
            return CustomHttpErrorResponse(MediaTypeConstants.MediaTypeErrorType, MediaTypeConstants.MediaTypeErrorMessage, HttpStatusCode.BadRequest);
        }
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
        CreatePaginationMetadata(nameof(GetAuthorsByQueryParametersAsync), getAllAuthorsResult.Value!);
        if (mediaTypeHeaderValue.MediaType.Equals(MediaTypeConstants.BookRentalManagerHateoasMediaType))
        {
            CollectionWithHateoasLinksDto collectionWithHateoasLinksDto = AddHateoasLinksToPaginatedCollection(
                nameof(GetAuthorsByQueryParametersAsync),
                queryParameters,
                _allowedRestMethodDtos,
                getAllAuthorsResult.Value!);
            return Ok(collectionWithHateoasLinksDto);
        }
        return Ok(getAllAuthorsResult.Value);
    }

    [HttpGet("{id}", Name = nameof(GetAuthorByIdAsync))]
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
        return Ok(AddHateoasLinks(_allowedRestMethodDtos, getAuthorByIdResult.Value!));
    }

    [HttpPost(Name = nameof(CreateAuthorAsync))]
    public async Task<ActionResult> CreateAuthorAsync(CreateAuthorCommand createAuthorCommand, CancellationToken cancellationToken)
    {
        Result<AuthorCreatedDto> createAuthorResult = await _dispatcher.DispatchAsync<AuthorCreatedDto>(
            createAuthorCommand,
            cancellationToken);
        if (!createAuthorResult.IsSuccess)
        {
            return HandleError(createAuthorResult);
        }
        return CreatedAtAction(
            nameof(GetAuthorByIdAsync),
            new { Id = createAuthorResult.Value!.Id },
            AddHateoasLinks(_allowedRestMethodDtos, createAuthorResult.Value!));
    }

    [HttpPatch("{id}/AddBooks", Name = nameof(AddExistingBooksToAuthor))]
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

    [HttpDelete("{id}", Name = nameof(DeleteAuthorByIdAsync))]
    public async Task<ActionResult> DeleteAuthorByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result deleteAuthorByIdResult = await _dispatcher.DispatchAsync(new DeleteAuthorByIdCommand(id), cancellationToken);
        if (!deleteAuthorByIdResult.IsSuccess)
        {
            return HandleError(deleteAuthorByIdResult);
        }
        return NoContent();
    }

    [HttpOptions("{id}/AddBooks")]
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
}
