using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.Queries;

namespace BookRentalManager.Api.Controllers.V1;

/// <summary>
/// Controller responsible for processing books.
/// </summary>
[ApiVersion("1.0")]
public sealed class BookController : ApiController
{
    private readonly IDispatcher _dispatcher;
    private readonly List<AllowedRestMethodsDto> _allowedRestMethodDtos;

    /// <summary>
    /// This class' controller.
    /// </summary>
    /// <param name="dispatcher"></param>
    public BookController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _allowedRestMethodDtos =
        [
            new(nameof(GetBookByIdAsync), "GET", "self"),
            new(nameof(PatchBookTitleEditionAndIsbnByIdAsync), "PATCH", "patch_book"),
            new(nameof(DeleteBookByIdAsync), "DELETE", "delete_book")
        ];
    }

    /// <summary>
    /// Gets all books based on the query parameters.
    /// </summary>
    /// <param name="queryParameters"></param>
    /// <param name="mediaType">
    /// Responsible for controlling the shape of the returned list of books.
    /// Choose between "application/json" and "application/vnd.bookrentalmanager.hateoas+json" in the response for the code 200.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>All the books based on the query parameters.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/v1/Book?pageIndex=2&amp;pageSize=2&amp;searchQuery=e&amp;sortBy=BookTitleDesc,CreatedAt
    /// 
    /// Using the HEAD HTTP verb to retrieve information about the endpoint:
    /// 
    ///     HEAD /api/v1/Book?pageIndex=2&amp;pageSize=2&amp;searchQuery=e&amp;sortBy=BookTitleDesc,CreatedAt
    /// 
    /// Sample response from GET using "application/vnd.bookrentalmanager.hateoas+json" as the "Accept" header:
    /// 
    ///     {
    ///       "values": [
    ///         {
    ///           "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///           "bookTitle": "string",
    ///           "authors": [
    ///             {
    ///               "fullName": "string"
    ///             }
    ///           ],
    ///           "edition": 0,
    ///           "isbn": "string",
    ///           "rentedAt": "datetime",
    ///           "dueDate": "datetime",
    ///           "rentedBy": {
    ///             "fullName": "string",
    ///             "email": "string"
    ///           },
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
    ///       "BookTitle",
    ///       "BookTitleDesc",
    ///       "Edition",
    ///       "EditionDesc",
    ///       "Isbn",
    ///       "IsbnDesc",
    ///       "RentedAt",
    ///       "RentedAtDesc",
    ///       "DueDate",
    ///       "DueDateDesc",
    ///       "CreatedAt",
    ///       "CreatedAtDesc"
    ///     ]
    /// </remarks>
    [HttpGet(Name = nameof(GetBooksByQueryParametersAsync))]
    [HttpHead]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Returns all the books based on the query parameters.",
        typeof(PaginatedList<GetBookDto>),
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
        Result<PaginatedList<GetBookDto>> getAllBooksResult = await _dispatcher.DispatchAsync(
                getBooksByQueryParametersQuery,
                cancellationToken);
        if (!getAllBooksResult.IsSuccess)
        {
            return HandleError(getAllBooksResult);
        }
        CreatePaginationMetadata(getAllBooksResult.Value!);
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

    /// <summary>
    /// Gets a book based on its ID.
    /// </summary>
    /// <param name="id">The book's ID.</param>
    /// <param name="mediaType">
    /// Responsible for controlling the shape of the returned book.
    /// Choose between "application/json" and "application/vnd.bookrentalmanager.hateoas+json" in the response for the code 200.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The book if it exists or an error if it does not.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/v1/Book/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// 
    /// Using the HEAD HTTP verb to retrieve information about the endpoint:
    /// 
    ///     HEAD /api/v1/Book/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// 
    /// Sample response from GET using "application/vnd.bookrentalmanager.hateoas+json" as the "Accept" header:
    /// 
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "bookTitle": "string",
    ///       "authors": [
    ///         {
    ///           "fullName": "string"
    ///         }
    ///       ],
    ///       "edition": 0,
    ///       "isbn": "string",
    ///       "rentedAt": datetime,
    ///       "dueDate": datetime,
    ///       "rentedBy": datetime,
    ///       "links": [
    ///         {
    ///           "href": "string",
    ///           "rel": "string",
    ///           "method": "string"
    ///         }
    ///       ]
    ///     }
    /// </remarks>
    [HttpGet("{id}", Name = nameof(GetBookByIdAsync))]
    [HttpHead("{id}")]
    [ActionName(nameof(GetBookByIdAsync))]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Returns the book based on its ID.",
        typeof(GetBookDto),
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "If the book does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<GetBookDto>> GetBookByIdAsync(
        Guid id,
        [FromHeader(Name = "Accept")] string? mediaType,
        CancellationToken cancellationToken)
    {
        var getBookByIdQuery = new GetBookByIdQuery(id);
        Result<GetBookDto> getBookByIdResult = await _dispatcher.DispatchAsync(getBookByIdQuery, cancellationToken);
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

    /// <summary>
    /// Creates new a book.
    /// </summary>
    /// <param name="createBookCommand"></param>
    /// <param name="mediaType">
    /// Responsible for controlling the shape of the returned body for the 201 response.
    /// Choose between "application/json" and "application/vnd.bookrentalmanager.hateoas+json" in the request body.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>201 if the book is created successfully or an error if not.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/Book
    ///     {
    ///       "authorIds": [
    ///         "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///       ],
    ///       "bookTitle": "Design Patterns: Elements of Reusable Object-Oriented Software",
    ///       "edition": 1,
    ///       "isbn": "0-201-63361-2"
    ///     }
    /// 
    /// Sample response using "application/vnd.bookrentalmanager.hateoas+json" as the "Content-Type" header:
    /// 
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "bookTitle": "string",
    ///       "edition": 0,
    ///       "isbn": "string",
    ///       "links": [
    ///         {
    ///           "href": "string",
    ///           "rel": "string",
    ///           "method": "string"
    ///         }
    ///       ]
    ///     }
    /// </remarks>
    [HttpPost(Name = nameof(CreateBookAsync))]
    [Consumes(
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "Returns the newly created book.",
        typeof(BookCreatedDto),
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)]
    [SwaggerResponse(
        StatusCodes.Status422UnprocessableEntity,
        "If any of the required fields is null or any validation errors happen.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> CreateBookAsync(
        CreateBookCommand createBookCommand,
        [FromHeader(Name = "Content-Type")] string? mediaType,
        CancellationToken cancellationToken)
    {
        Result<BookCreatedDto> createBookResult = await _dispatcher.DispatchAsync(createBookCommand, cancellationToken);
        if (!createBookResult.IsSuccess)
        {
            return HandleError(createBookResult);
        }
        if (IsMediaTypeVendorSpecific(mediaType))
        {
            return CreatedAtAction(
                nameof(GetBookByIdAsync),
                new { createBookResult.Value!.Id },
                AddHateoasLinks(_allowedRestMethodDtos, createBookResult.Value));
        }
        return CreatedAtAction(nameof(GetBookByIdAsync) , new { createBookResult.Value!.Id }, createBookResult.Value);
    }

    /// <summary>
    /// Updates a book's title, edition and/or ISBN fields.
    /// </summary>
    /// <param name="id">The book's ID.</param>
    /// <param name="patchBookTitleEditionAndIsbnByIdDtoPatchDocument"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>204 if the fields are successfully updated or an error if not.</returns>
    /// <remarks>
    /// Sample request replacing all fields:
    /// 
    ///     PATCH /api/v1/Book/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     [
    ///       {
    ///         "op": "replace",
    ///         "path": "/bookTitle",
    ///         "value": "Clean Code: A Handbook of Agile Software Craftsmanship"
    ///       },
    ///       {
    ///         "op": "replace",
    ///         "path": "/edition",
    ///         "value": "2"
    ///       },
    ///       {
    ///         "op": "replace",
    ///         "path": "/isbn",
    ///         "value": "978-0132350884"
    ///       }
    ///     ]
    /// </remarks>
    [HttpPatch("{id}", Name = nameof(PatchBookTitleEditionAndIsbnByIdAsync))]
    [Consumes(CustomMediaTypeNames.Application.JsonPatchJson)]
    [SwaggerResponse(StatusCodes.Status204NoContent, "If the patch operation was successful.")]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "If the book does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "If the JSON patch document is malformed or any validation errors happen.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
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

    /// <summary>
    /// Deletes a book based on its ID.
    /// </summary>
    /// <param name="id">The book's ID.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>204 if the book is successfully deleted or an error if not.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/v1/Book/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    [HttpDelete("{id}", Name = nameof(DeleteBookByIdAsync))]
    [SwaggerResponse(StatusCodes.Status204NoContent, "If the delete operation was successful.")]
    [SwaggerResponse(StatusCodes.Status404NotFound,
        "If the book does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status422UnprocessableEntity,
        "If the book is currently rented by a customer.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> DeleteBookByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result deleteBookByIdResult = await _dispatcher.DispatchAsync(new DeleteBookByIdCommand(id), cancellationToken);
        if (!deleteBookByIdResult.IsSuccess)
        {
            return HandleError(deleteBookByIdResult);
        }
        return NoContent();
    }

    /// <summary>
    /// Gets all of the allowed operations for the /api/v1/Book endpoints.
    /// </summary>
    /// <returns>A list of the allowed operations for the /api/v1/Book endpoints.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     OPTIONS /api/v1/Book
    /// </remarks>
    [HttpOptions]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all of the options in the \"Allow\" response header.")]
    public ActionResult GetBookOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PATCH, DELETE, OPTIONS");
        return Ok();
    }
}
