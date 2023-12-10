using BookRentalManager.Application.Authors.Commands;
using BookRentalManager.Application.Authors.Queries;

namespace BookRentalManager.Api.Controllers.V1;

/// <summary>
/// Controller responsible for processing authors.
/// </summary>
[ApiVersion("1.0")]
public sealed class AuthorController : ApiController
{
    private readonly IDispatcher _dispatcher;
    private readonly List<AllowedRestMethodsDto> _allowedRestMethodDtos;

    /// <summary>
    /// This class' constructor.
    /// </summary>
    /// <param name="dispatcher"></param>
    public AuthorController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _allowedRestMethodDtos =
        [
            new(nameof(GetAuthorByIdAsync), "GET", "self"),
            new(nameof(AddExistingBooksToAuthor), "PATCH", "add_existing_books_to_author"),
            new(nameof(DeleteAuthorByIdAsync), "DELETE", "delete_author")
        ];
    }

    /// <summary>
    /// Gets all authors based on the query parameters.
    /// </summary>
    /// <param name="queryParameters"></param>
    /// <param name="mediaType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>All the authors based on the query parameters.</returns>
    /// <remarks>
    /// Example:
    /// 
    ///     GET/Author?pageSize=1&amp;pageIndex=3&amp;sortBy=FullNameDesc,CreatedAt&amp;searchQuery=Edgar
    /// 
    /// Allowed to sort by:
    /// 
    ///     [
    ///         "FullName",
    ///         "FullNameDesc",
    ///         "CreatedAt",
    ///         "CreatedAtDesc"
    ///     ]
    /// </remarks>
    /// <response code="200">Returns all the authors based on the query parameters.</response>
    /// <response code="422">If any of the query parameters does not exist.</response>
    [HttpGet(Name = nameof(GetAuthorsByQueryParametersAsync))]
    [HttpHead]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PaginatedList<GetAuthorDto>>> GetAuthorsByQueryParametersAsync(
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        [FromHeader(Name = "Accept")] string? mediaType,
        CancellationToken cancellationToken)
    {
        var getAuthorsByQueryParametersQuery = new GetAuthorsByQueryParametersQuery(
            queryParameters.PageIndex,
            queryParameters.PageSize,
            queryParameters.SearchQuery,
            queryParameters.SortBy);
        Result<PaginatedList<GetAuthorDto>> getAllAuthorsResult = await _dispatcher.DispatchAsync(
                getAuthorsByQueryParametersQuery,
                cancellationToken);
        if (!getAllAuthorsResult.IsSuccess)
        {
            return HandleError(getAllAuthorsResult);
        }
        CreatePaginationMetadata(getAllAuthorsResult.Value!);
        if (IsMediaTypeVendorSpecific(mediaType))
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

    /// <summary>
    /// Gets an author based on their ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="mediaType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The author if they exist or an error if they do not.</returns>
    /// <remarks>
    /// Example:
    /// 
    ///     GET/Author/01426685-8ee0-40b0-8039-57456082ee84
    /// </remarks>
    /// <response code="200">Returns the author based on their ID.</response>
    /// <response code="404">If the author does not exist.</response>
    [HttpGet("{id}", Name = nameof(GetAuthorByIdAsync))]
    [HttpHead("{id}")]
    [ActionName(nameof(GetAuthorByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetAuthorDto>> GetAuthorByIdAsync(
        Guid id,
        [FromHeader(Name = "Accept")] string? mediaType,
        CancellationToken cancellationToken)
    {
        Result<GetAuthorDto> getAuthorByIdResult = await _dispatcher.DispatchAsync(
            new GetAuthorByIdQuery(id),
            cancellationToken);
        if (!getAuthorByIdResult.IsSuccess)
        {
            return HandleError(getAuthorByIdResult);
        }
        if (IsMediaTypeVendorSpecific(mediaType))
        {
            return Ok(AddHateoasLinks(_allowedRestMethodDtos, getAuthorByIdResult.Value!));
        }
        return Ok(getAuthorByIdResult.Value);
    }

    /// <summary>
    /// Creates a new author.
    /// </summary>
    /// <param name="createAuthorCommand"></param>
    /// <param name="mediaType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>201 if the author is created successfully or an error if not.</returns>
    /// <remarks>
    /// Example:
    /// 
    ///     POST /Author
    ///     {
    ///         "firstName": "John",
    ///         "lastName": "Doe"
    ///     }
    /// </remarks>
    /// <response code="201">Returns the newly created author.</response>
    /// <response code="422">If any of the required fields is null.</response>
    [HttpPost(Name = nameof(CreateAuthorAsync))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> CreateAuthorAsync(
        CreateAuthorCommand createAuthorCommand,
        [FromHeader(Name = "Accept")] string? mediaType,
        CancellationToken cancellationToken)
    {
        Result<AuthorCreatedDto> createAuthorResult = await _dispatcher.DispatchAsync(
            createAuthorCommand,
            cancellationToken);
        if (!createAuthorResult.IsSuccess)
        {
            return HandleError(createAuthorResult);
        }
        if (IsMediaTypeVendorSpecific(mediaType))
        {
            return CreatedAtAction(
                nameof(GetAuthorByIdAsync),
                new { createAuthorResult.Value!.Id },
                AddHateoasLinks(_allowedRestMethodDtos, createAuthorResult.Value!));
        }
        return CreatedAtAction(nameof(GetAuthorByIdAsync), new { createAuthorResult.Value!.Id }, createAuthorResult.Value);
    }

    /// <summary>
    /// Adds an existing book to an existing author.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="patchAuthorBooksDtoPatchDocument"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>204 if the book is successfully added to the author or an error if not.</returns>
    /// <remarks>
    /// Example:
    /// 
    ///     PATCH /Author/9949d99a-0459-49bc-a808-e40b8e294ecd/AddBooks
    ///     {
    ///         "op": "add",
    ///         "path": "/bookIds",
    ///         "value": [
    ///             "660e76c2-3028-4e65-92c9-8845c233456b",
    ///             "e251bb4d-f9d2-4e34-9ada-070610bad82e"
    ///         ]
    ///     }
    /// </remarks>
    /// <response code="204">If the patch operation was successful.</response>
    /// <response code="400">If any of the patch operations is incorrect.</response>
    /// <response code="404">If any of the books or the author does not exist.</response>
    [HttpPatch("{id}/AddBooks", Name = nameof(AddExistingBooksToAuthor))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Deletes an author based on their ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>204 if the book is successfully delete or an error if not.</returns>
    /// <remarks>
    /// Example:
    /// 
    ///     DELETE /Author/9949d99a-0459-49bc-a808-e40b8e294ecd
    /// </remarks>
    /// <response code="204">If the delete operation was successful.</response>
    /// <response code="404">If the author does not exist.</response>
    [HttpDelete("{id}", Name = nameof(DeleteAuthorByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAuthorByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result deleteAuthorByIdResult = await _dispatcher.DispatchAsync(new DeleteAuthorByIdCommand(id), cancellationToken);
        if (!deleteAuthorByIdResult.IsSuccess)
        {
            return HandleError(deleteAuthorByIdResult);
        }
        return NoContent();
    }

    /// <summary>
    /// Gets all of the allowed operations for the /Author endpoint.
    /// </summary>
    /// <returns>A list of the allowed operations for the /Author endpoint.</returns>
    /// <remarks>
    /// Example:
    /// 
    ///     OPTIONS /Author
    /// </remarks>
    /// <response code="200">Returns all of the options in the "Allow" response header.</response>
    [HttpOptions]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetAuthorOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, DELETE, OPTIONS");
        return Ok();
    }

    /// <summary>
    /// Gets all of the allowed operations for the /Author/{authorId}/AddBooks endpoint.
    /// </summary>
    /// <returns>A list of the allowed operations for the /Author/{authorId}/AddBooks endpoint.</returns>
    /// <remarks>
    /// Example:
    /// 
    ///     OPTIONS /Author/01426685-8ee0-40b0-8039-57456082ee84/AddBook
    /// </remarks>
    /// <response code="200">Returns all of the options in the "Allow" response header.</response>
    /// <response code="404">If the author does not exist.</response>
    [HttpOptions("{id}/AddBooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetAuthorAddBooksOptionsAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<GetAuthorDto> getAuthorByIdResult = await _dispatcher.DispatchAsync(
            new GetAuthorByIdQuery(id),
            cancellationToken);
        if (!getAuthorByIdResult.IsSuccess)
        {
            return HandleError(getAuthorByIdResult);
        }
        Response.Headers.Append("Allow", "PATCH, OPTIONS");
        return Ok();
    }
}
