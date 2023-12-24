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
    /// <param name="mediaType">
    /// Responsible for controlling the shape of the returned list of authors.
    /// Choose between "application/json" and "application/vnd.bookrentalmanager.hateoas+json" in the response for the code 200.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>All the authors based on the query parameters.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/v1/Author?pageIndex=2&amp;pageSize=2&amp;searchQuery=e&amp;sortBy=FullNameDesc,CreatedAt
    /// 
    /// Using the HEAD HTTP verb to retrieve information about the endpoint:
    /// 
    ///     HEAD /api/v1/Author?pageIndex=2&amp;pageSize=2&amp;searchQuery=e&amp;sortBy=FullNameDesc,CreatedAt
    /// 
    /// Sample response from GET using "application/vnd.bookrentalmanager.hateoas+json" as the "Accept" header:
    /// 
    ///     {
    ///       "values": [
    ///         {
    ///           "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///           "fullName": "string",
    ///           "books": [
    ///             {
    ///               "bookTitle": "string",
    ///               "edition": 0,
    ///               "isbn": "string"
    ///             }
    ///           ],
    ///           "links": [
    ///             {
    ///               "href": "string",
    ///               "rel": "string",
    ///               "method": "string"
    ///             }
    ///           ]
    ///         }
    ///       ],
    ///       "links": [
    ///         {
    ///           "href": "string",
    ///           "rel": "string",
    ///           "method": "string"
    ///         }
    ///       ]
    ///     }
    /// 
    /// Allowed to sort by:
    /// 
    ///     [
    ///       "FullName",
    ///       "FullNameDesc",
    ///       "CreatedAt",
    ///       "CreatedAtDesc"
    ///     ]
    /// </remarks>
    [HttpGet(Name = nameof(GetAuthorsByQueryParametersAsync))]
    [HttpHead]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Returns all the authors based on the query parameters.",
        typeof(PaginatedList<GetAuthorDto>),
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "If any of the query parameters' types is incorrect.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status422UnprocessableEntity,
        "If any of the query parameters does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
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
    /// <param name="id">The author's ID.</param>
    /// <param name="mediaType">
    /// Responsible for controlling the shape of the returned author.
    /// Choose between "application/json" and "application/vnd.bookrentalmanager.hateoas+json" in the response for the code 200.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>The author if they exist or an error if they do not.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/v1/Author/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// 
    /// Using the HEAD HTTP verb to retrieve information about the endpoint:
    /// 
    ///     HEAD /api/v1/Author/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// 
    /// Sample response from GET using "application/vnd.bookrentalmanager.hateoas+json" as the "Accept" header:
    /// 
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "fullName": "string",
    ///       "books": [
    ///         {
    ///           "bookTitle": "string",
    ///           "edition": 0,
    ///           "isbn": "string"
    ///         }
    ///       ],
    ///       "links": [
    ///         {
    ///           "href": "string",
    ///           "rel": "string",
    ///           "method": "string"
    ///         }
    ///       ]
    ///     }
    /// </remarks>
    [HttpGet("{id}", Name = nameof(GetAuthorByIdAsync))]
    [HttpHead("{id}")]
    [ActionName(nameof(GetAuthorByIdAsync))]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Returns the author based on their ID.",
        typeof(GetAuthorDto),
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "If the author does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
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
    /// <param name="mediaType">
    /// Responsible for controlling the shape of the returned body for the 201 response.
    /// Choose between "application/json" and "application/vnd.bookrentalmanager.hateoas+json" in the request body.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>201 if the author is created successfully or an error if not.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/Author
    ///     {
    ///       "firstName": "John",
    ///       "lastName": "Doe"
    ///     }
    /// 
    /// Sample response using "application/vnd.bookrentalmanager.hateoas+json" as the "Content-Type" header:
    /// 
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "firstName": "string",
    ///       "lastName": "string",
    ///       "links": [
    ///         {
    ///           "href": "string",
    ///           "rel": "string",
    ///           "method": "string"
    ///         }
    ///       ]
    ///     }
    /// </remarks>
    [HttpPost(Name = nameof(CreateAuthorAsync))]
    [Consumes(
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "Returns the newly created author.",
        typeof(AuthorCreatedDto),
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)]
    [SwaggerResponse(
        StatusCodes.Status422UnprocessableEntity,
        "If any of the required fields is null or any validation errors happen.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<AuthorCreatedDto>> CreateAuthorAsync(
        CreateAuthorCommand createAuthorCommand,
        [FromHeader(Name = "Content-Type")] string? mediaType,
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
    /// <param name="id">The author's ID.</param>
    /// <param name="patchAuthorBooksDtoPatchDocument"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>204 if the book is successfully added to the author or an error if not.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PATCH /api/v1/Author/3fa85f64-5717-4562-b3fc-2c963f66afa6/AddBooks
    ///     [
    ///       {
    ///         "op": "add",
    ///         "path": "/bookIds",
    ///         "value": [
    ///             "660e76c2-3028-4e65-92c9-8845c233456b",
    ///             "e251bb4d-f9d2-4e34-9ada-070610bad82e"
    ///         ]
    ///       }
    ///     ]
    /// 
    /// Put the book IDs that will be added to the author in the "value" property.
    /// </remarks>
    [HttpPatch("{id}/AddBooks", Name = nameof(AddExistingBooksToAuthor))]
    [Consumes(CustomMediaTypeNames.Application.JsonPatchJson)]
    [SwaggerResponse(StatusCodes.Status204NoContent, "If the patch operation was successful.")]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "If any of the books or the author does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "If the JSON patch document is malformed or any validation errors happen.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
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
    /// <param name="id">The author's ID.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>204 if the author is successfully deleted or an error if not.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/v1/Author/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    [HttpDelete("{id}", Name = nameof(DeleteAuthorByIdAsync))]
    [SwaggerResponse(StatusCodes.Status204NoContent, "If the delete operation was successful.")]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "If the author does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status422UnprocessableEntity,
        "If the author has books.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
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
    /// Gets all of the allowed operations for the /api/v1/Author endpoint.
    /// </summary>
    /// <returns>A list of the allowed operations for the /api/v1/Author endpoints.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     OPTIONS /api/v1/Author
    /// </remarks>
    [HttpOptions]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all of the options in the \"Allow\" response header.")]
    public ActionResult GetAuthorOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, DELETE, OPTIONS");
        return Ok();
    }

    /// <summary>
    /// Gets all of the allowed operations for the /api/v1/Author/{authorId}/AddBooks endpoints.
    /// </summary>
    /// <returns>A list of the allowed operations for the /api/v1/Author/{authorId}/AddBooks endpoints.</returns>
    /// <param name="id">The author's ID.</param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     OPTIONS /api/v1/Author/3fa85f64-5717-4562-b3fc-2c963f66afa6/AddBook
    /// </remarks>
    [HttpOptions("{id}/AddBooks")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all of the options in the \"Allow\" response header.")]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "If the author does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
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
