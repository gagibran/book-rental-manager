using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.Queries;

namespace BookRentalManager.Api.Controllers.V1;

[ApiVersion("1.0")]
public sealed class BookController : ApiController
{
    private readonly IDispatcher _dispatcher;
    private readonly List<AllowedRestMethodsDto> _allowedRestMethodDtos;

    public BookController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _allowedRestMethodDtos = new List<AllowedRestMethodsDto>
        {
            new AllowedRestMethodsDto(nameof(GetBookByIdAsync), "GET", "self"),
            new AllowedRestMethodsDto(nameof(PatchBookTitleEditionAndIsbnByIdAsync), "PATCH", "patch_book"),
            new AllowedRestMethodsDto(nameof(DeleteBookByIdAsync), "DELETE", "delete_book")
        };
    }

    [HttpGet(Name = nameof(GetBooksByQueryParametersAsync))]
    [HttpHead]
    public async Task<ActionResult<PaginatedList<GetBookDto>>> GetBooksByQueryParametersAsync(
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        [FromHeader(Name = "Accept")] string? mediaType,
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
        CreatePaginationMetadata(nameof(GetBooksByQueryParametersAsync), getAllBooksResult.Value!);
        if (IsMediaTypeVendorSpecific(mediaType))
        {
            CollectionWithHateoasLinksDto collectionWithHateoasLinksDto = AddHateoasLinksToPaginatedCollection(
                nameof(GetBooksByQueryParametersAsync),
                queryParameters,
                _allowedRestMethodDtos,
                getAllBooksResult.Value!);
            return Ok(collectionWithHateoasLinksDto);
        }
        return Ok(getAllBooksResult.Value);
    }

    [HttpGet("{id}", Name = nameof(GetBookByIdAsync))]
    [HttpHead("{id}")]
    [ActionName(nameof(GetBookByIdAsync))]
    public async Task<ActionResult<GetBookDto>> GetBookByIdAsync(
        Guid authorId,
        Guid id,
        [FromHeader(Name = "Accept")] string? mediaType,
        CancellationToken cancellationToken)
    {
        var getBookByIdQuery = new GetBookByIdQuery(id);
        Result<GetBookDto> getBookByIdResult = await _dispatcher.DispatchAsync<GetBookDto>(getBookByIdQuery, cancellationToken);
        if (!getBookByIdResult.IsSuccess)
        {
            return HandleError(getBookByIdResult);
        }
        if (IsMediaTypeVendorSpecific(mediaType))
        {
            return Ok(AddHateoasLinks(_allowedRestMethodDtos, getBookByIdResult.Value!));
        }
        return Ok(getBookByIdResult.Value);
    }

    [HttpPost(Name = nameof(CreateBookAsync))]
    public async Task<ActionResult> CreateBookAsync(
        CreateBookCommand createBookCommand,
        [FromHeader(Name = "Accept")] string? mediaType,
        CancellationToken cancellationToken)
    {
        Result<BookCreatedDto> createBookResult = await _dispatcher.DispatchAsync<BookCreatedDto>(createBookCommand, cancellationToken);
        if (!createBookResult.IsSuccess)
        {
            return HandleError(createBookResult);
        }
        if (IsMediaTypeVendorSpecific(mediaType))
        {
            return CreatedAtAction(
                nameof(GetBookByIdAsync),
                new { Id = createBookResult.Value!.Id },
                AddHateoasLinks(_allowedRestMethodDtos, createBookResult.Value));
        }
        return CreatedAtAction(nameof(GetBookByIdAsync), new { Id = createBookResult.Value!.Id }, _allowedRestMethodDtos);
    }

    [HttpPatch("{id}", Name = nameof(PatchBookTitleEditionAndIsbnByIdAsync))]
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

    [HttpDelete("{id}", Name = nameof(DeleteBookByIdAsync))]
    public async Task<ActionResult> DeleteBookByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result deleteBookByIdResult = await _dispatcher.DispatchAsync(new DeleteBookByIdCommand(id), cancellationToken);
        if (!deleteBookByIdResult.IsSuccess)
        {
            return HandleError(deleteBookByIdResult);
        }
        return NoContent();
    }

    [HttpGet("ExcludingAuthor/{authorId}")]
    [HttpHead("ExcludingAuthor/{authorId}")]
    public async Task<ActionResult<PaginatedList<GetBookDto>>> GetBooksByQueryParametersExcludingFromAuthorAsync(
        Guid authorId,
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        [FromHeader(Name = "Accept")] string? mediaType,
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
        CreatePaginationMetadata(nameof(GetBooksByQueryParametersExcludingFromAuthorAsync), getAllBooksResult.Value!);
        if (IsMediaTypeVendorSpecific(mediaType))
        {
            CollectionWithHateoasLinksDto collectionWithHateoasLinksDto = AddHateoasLinksToPaginatedCollection(
                nameof(GetBooksByQueryParametersExcludingFromAuthorAsync),
                queryParameters,
                _allowedRestMethodDtos,
                getAllBooksResult.Value!);
            return Ok(collectionWithHateoasLinksDto);
        }
        return Ok(getAllBooksResult.Value);
    }

    [HttpOptions]
    public ActionResult GetBookOptions()
    {
        Response.Headers.Add("Allow", "GET, HEAD, POST, PATCH, DELETE, OPTIONS");
        return Ok();
    }

    [HttpOptions("ExcludingAuthor")]
    public ActionResult GetExcludingAuthorOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS");
        return Ok();
    }
}
