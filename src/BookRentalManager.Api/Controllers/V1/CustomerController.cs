using BookRentalManager.Application.Customers.Commands;
using BookRentalManager.Application.Customers.Queries;

namespace BookRentalManager.Api.Controllers.V1;

/// <summary>
/// Controller responsible for processing customers.
/// </summary>
[ApiVersion("1.0")]
public sealed class CustomerController : ApiController
{
    private readonly IDispatcher _dispatcher;
    private readonly List<AllowedRestMethodsDto> _allowedRestMethodDtos;

    /// <summary>
    /// This class' controller.
    /// </summary>
    /// <param name="dispatcher"></param>
    public CustomerController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _allowedRestMethodDtos =
        [
            new(nameof(GetCustomerByIdAsync), "GET", "self"),
            new(nameof(PatchCustomerNameAndPhoneNumberByIdAsync), "PATCH", "patch_customer"),
            new("RentBooks", "PATCH", "rent_books"),
            new("ReturnBooks", "PATCH", "return_books"),
            new(nameof(DeleteCustomerByIdAsync), "DELETE", "delete_customer")
        ];
    }

    /// <summary>
    /// Gets all customers based on the query parameters.
    /// </summary>
    /// <param name="queryParameters"></param>
    /// <param name="mediaType">
    /// Responsible for controlling the shape of the returned list of customers.
    /// Choose between "application/json" and "application/vnd.bookrentalmanager.hateoas+json" in the response for the code 200.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>All the customers based on the query parameters.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/v1/Customers?pageIndex=2&amp;pageSize=2&amp;searchQuery=e&amp;sortBy=FullNameDesc,CreatedAt
    /// 
    /// Using the HEAD HTTP verb to retrieve information about the endpoint:
    /// 
    ///     HEAD /api/v1/Customers?pageIndex=2&amp;pageSize=2&amp;searchQuery=e&amp;sortBy=FullNameDesc,CreatedAt
    /// 
    /// Sample response from GET using "application/vnd.bookrentalmanager.hateoas+json" as the "Accept" header:
    /// 
    ///     {
    ///       "values": [
    ///         {
    ///           "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///           "fullName": "string",
    ///           "email": "string",
    ///           "phoneNumber": "string",
    ///           "books": [
    ///             {
    ///               "bookTitle": "string",
    ///               "edition": 0,
    ///               "isbn": "string",
    ///               "rentedAt": "datetime",
    ///               "dueDate": "datetime"
    ///             }
    ///           ],
    ///           "customerStatus": "string",
    ///           "customerPoints": 0,
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
    ///       "Email",
    ///       "EmailDesc",
    ///       "PhoneNumber",
    ///       "PhoneNumberDesc",
    ///       "CustomerStatus",
    ///       "CustomerStatusDesc",
    ///       "CustomerPoints",
    ///       "CustomerPointsDesc",
    ///       "CreatedAt",
    ///       "CreatedAtDesc"
    ///     ]
    /// </remarks>
    [HttpGet(Name = nameof(GetCustomersByQueryParametersAsync))]
    [HttpHead]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Returns all the customers based on the query parameters.",
        typeof(PaginatedList<GetCustomerDto>),
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
    public async Task<ActionResult<PaginatedList<GetCustomerDto>>> GetCustomersByQueryParametersAsync(
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        [FromHeader(Name = "Accept")] string? mediaType,
        CancellationToken cancellationToken)
    {
        var getCustomersByQueryParametersQuery = new GetCustomersByQueryParametersQuery(
            queryParameters.PageIndex,
            queryParameters.PageSize,
            queryParameters.SearchQuery,
            queryParameters.SortBy);
        Result<PaginatedList<GetCustomerDto>> getAllCustomersResult = await _dispatcher.DispatchAsync(
            getCustomersByQueryParametersQuery,
            cancellationToken);
        if (!getAllCustomersResult.IsSuccess)
        {
            return HandleError(getAllCustomersResult);
        }
        CreatePaginationMetadata(getAllCustomersResult.Value!);
        if (IsMediaTypeVendorSpecific(mediaType))
        {
            CollectionWithHateoasLinksDto collectionWithHateoasLinksDto = AddHateoasLinksToPaginatedCollection(
                nameof(GetCustomersByQueryParametersAsync),
                queryParameters,
                _allowedRestMethodDtos,
                getAllCustomersResult.Value!);
            return Ok(collectionWithHateoasLinksDto);
        }
        return Ok(getAllCustomersResult.Value);
    }

    /// <summary>
    /// Gets a customer based on their ID.
    /// </summary>
    /// <param name="id">The customer's ID.</param>
    /// <param name="mediaType">
    /// Responsible for controlling the shape of the returned customer.
    /// Choose between "application/json" and "application/vnd.bookrentalmanager.hateoas+json" in the response for the code 200.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The customer if they exist or an error if they do not.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/v1/Customer/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// 
    /// Using the HEAD HTTP verb to retrieve information about the endpoint:
    /// 
    ///     HEAD /api/v1/Customer/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// 
    /// Sample response from GET using "application/vnd.bookrentalmanager.hateoas+json" as the "Accept" header:
    /// 
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "fullName": "string",
    ///       "email": "string",
    ///       "phoneNumber": "string",
    ///       "books": [
    ///         {
    ///           "bookTitle": "string",
    ///           "edition": 0,
    ///           "isbn": "string",
    ///           "rentedAt": "datetime",
    ///           "dueDate": "datetime"
    ///         }
    ///       ],
    ///       "customerStatus": "string",
    ///       "customerPoints": 0,
    ///       "links": [
    ///         {
    ///           "href": "string",
    ///           "rel": "string",
    ///           "method": "string"
    ///         }
    ///       ]
    ///     }
    /// </remarks>
    [HttpGet("{id}", Name = nameof(GetCustomerByIdAsync))]
    [HttpHead("{id}")]
    [ActionName(nameof(GetCustomerByIdAsync))]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Returns the customer based on their ID.",
        typeof(GetCustomerDto),
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "If the customer does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<GetCustomerDto>> GetCustomerByIdAsync(
        Guid id,
        [FromHeader(Name = "Accept")] string? mediaType,
        CancellationToken cancellationToken)
    {
        Result<GetCustomerDto> getCustomerByIdResult = await _dispatcher.DispatchAsync(
            new GetCustomerByIdQuery(id),
            cancellationToken);
        if (!getCustomerByIdResult.IsSuccess)
        {
            return HandleError(getCustomerByIdResult);
        }
        if (IsMediaTypeVendorSpecific(mediaType))
        {
            return Ok(AddHateoasLinks(_allowedRestMethodDtos, getCustomerByIdResult.Value!));
        }
        return Ok(getCustomerByIdResult.Value);
    }

    /// <summary>
    /// Creates new a customer.
    /// </summary>
    /// <param name="createCustomerCommand"></param>
    /// <param name="mediaType">
    /// Responsible for controlling the shape of the returned body for the 201 response.
    /// Choose between "application/json" and "application/vnd.bookrentalmanager.hateoas+json" in the request body.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>201 if the customer is created successfully or an error if not.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/Customer
    ///     {
    ///       "firstName": "Jane",
    ///       "lastName": "Doe",
    ///       "email": "jane.doe@email.com",
    ///       "phoneNumber": {
    ///         "areaCode": "834",
    ///         "prefixAndLineNumber": "4552897"
    ///       }
    ///     }
    /// 
    /// Sample response using "application/vnd.bookrentalmanager.hateoas+json" as the "Content-Type" header:
    /// 
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "firstName": "string",
    ///       "lastName": "string",
    ///       "email": "string",
    ///       "areaCode": 0,
    ///       "prefixAndLineNumber": 0,
    ///       "customerStatus": "string",
    ///       "customerPoints": 0,
    ///       "links": [
    ///         {
    ///           "href": "string",
    ///           "rel": "string",
    ///           "method": "string"
    ///         }
    ///       ]
    ///     }
    /// </remarks>
    [HttpPost]
    [Consumes(
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "Returns the newly created customer.",
        typeof(CustomerCreatedDto),
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)]
    [SwaggerResponse(
        StatusCodes.Status422UnprocessableEntity,
        "If any of the required fields is null or any validation errors happen.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> CreateCustomerAsync(
        CreateCustomerCommand createCustomerCommand,
        [FromHeader(Name = "Content-Type")] string? mediaType,
        CancellationToken cancellationToken)
    {
        Result<CustomerCreatedDto> createCustomerResult = await _dispatcher.DispatchAsync(
            createCustomerCommand,
            cancellationToken);
        if (!createCustomerResult.IsSuccess)
        {
            return HandleError(createCustomerResult);
        }
        if (IsMediaTypeVendorSpecific(mediaType))
        {
            return CreatedAtAction(
                nameof(GetCustomerByIdAsync),
                new { createCustomerResult.Value!.Id },
                AddHateoasLinks(_allowedRestMethodDtos, createCustomerResult.Value));
        }
        return CreatedAtAction(nameof(GetCustomerByIdAsync), new { createCustomerResult.Value!.Id }, createCustomerResult.Value);
    }

    /// <summary>
    /// Updates a customers's first name, last name and/or phone number fields.
    /// </summary>
    /// <param name="id">The customer's ID.</param>
    /// <param name="patchCustomerNameAndPhoneNumberDtoPatchDocument"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>204 if the fields are successfully updated or an error if not.</returns>
    /// <remarks>
    /// Sample request replacing all fields:
    /// 
    ///     PATCH /api/v1/Customer/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     [
    ///         {
    ///             "op": "replace",
    ///             "path": "/firstname",
    ///             "value": "John"
    ///         },
    ///             {
    ///             "op": "replace",
    ///             "path": "/lastname",
    ///             "value": "James"
    ///         },
    ///         {
    ///             "op": "replace",
    ///             "path": "/areacode",
    ///             "value": "322"
    ///         }
    ///     ]
    /// </remarks>
    [HttpPatch("{id}", Name = nameof(PatchCustomerNameAndPhoneNumberByIdAsync))]
    [Consumes(CustomMediaTypeNames.Application.JsonPatchJson)]
    [SwaggerResponse(StatusCodes.Status204NoContent, "If the patch operation was successful.")]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "If the customer does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "If the JSON patch document is malformed or any validation errors happen.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> PatchCustomerNameAndPhoneNumberByIdAsync(
        Guid id,
        JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto> patchCustomerNameAndPhoneNumberDtoPatchDocument,
        CancellationToken cancellationToken)
    {
        var patchCustomerNameAndPhoneNumberByIdCommand = new PatchCustomerNameAndPhoneNumberByIdCommand(id, patchCustomerNameAndPhoneNumberDtoPatchDocument);
        Result patchCustomerNameAndPhoneNumberResult = await _dispatcher.DispatchAsync(patchCustomerNameAndPhoneNumberByIdCommand, cancellationToken);
        if (!patchCustomerNameAndPhoneNumberResult.IsSuccess)
        {
            return HandleError(patchCustomerNameAndPhoneNumberResult);
        }
        return NoContent();
    }

    /// <summary>
    /// Changes the current books a customer has rented.
    /// </summary>
    /// <param name="id">The customer's ID.</param>
    /// <param name="changeCustomerBooksByBookIdsDtoPatchDocument"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>204 if the patch operation is successful or an error if it is not.</returns>
    /// <remarks>
    /// Sample request for renting books:
    /// 
    ///     PATCH /api/v1/Customer/3fa85f64-5717-4562-b3fc-2c963f66afa6/RentBooks
    ///     [
    ///       {
    ///         "op": "add",
    ///         "path": "/bookIds",
    ///         "value": [
    ///           "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///         ]
    ///       }
    ///     ]
    /// 
    /// Sample request for returning rented books:
    /// 
    ///     PATCH /api/v1/Customer/3fa85f64-5717-4562-b3fc-2c963f66afa6/ReturnBooks
    ///     [
    ///       {
    ///         "op": "add",
    ///         "path": "/bookIds",
    ///         "value": [
    ///           "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///         ]
    ///       }
    ///     ]
    /// </remarks>
    [HttpPatch("{id}/RentBooks", Name = "RentBooks")]
    [HttpPatch("{id}/ReturnBooks", Name = "ReturnBooks")]
    [Consumes(CustomMediaTypeNames.Application.JsonPatchJson)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "If the customer does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "If the JSON patch document is malformed or any validation errors happen.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> ChangeCustomerBooksByBookIdsAsync(
        Guid id,
        JsonPatchDocument<ChangeCustomerBooksByBookIdsDto> changeCustomerBooksByBookIdsDtoPatchDocument,
        CancellationToken cancellationToken)
    {
        bool isReturn = Request.Path.Value!.Contains("return", StringComparison.OrdinalIgnoreCase);
        var changeCustomerBooksByBookIdsCommand = new ChangeCustomerBooksByBookIdsCommand(
            id,
            changeCustomerBooksByBookIdsDtoPatchDocument,
            isReturn);
        Result changeCustomerBooksByBookIdsResult = await _dispatcher.DispatchAsync(changeCustomerBooksByBookIdsCommand, cancellationToken);
        if (!changeCustomerBooksByBookIdsResult.IsSuccess)
        {
            return HandleError(changeCustomerBooksByBookIdsResult);
        }
        return NoContent();
    }

    /// <summary>
    /// Deletes a customer based on their ID.
    /// </summary>
    /// <param name="id">The customer's ID.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>204 if the customer is successfully deleted or an error if not.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/v1/Book/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    [HttpDelete("{id}", Name = nameof(DeleteCustomerByIdAsync))]
    [SwaggerResponse(StatusCodes.Status204NoContent, "If the delete operation was successful.")]
    [SwaggerResponse(StatusCodes.Status404NotFound,
        "If the customer does not exist.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status422UnprocessableEntity,
        "If the customer currently has any rented books.",
        typeof(ValidationProblemDetails),
        CustomMediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> DeleteCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleteCustomerByIdCommand = new DeleteCustomerByIdCommand(id);
        Result deleteCustomerByIdResult = await _dispatcher.DispatchAsync(deleteCustomerByIdCommand, cancellationToken);
        if (!deleteCustomerByIdResult.IsSuccess)
        {
            return HandleError(deleteCustomerByIdResult);
        }
        return NoContent();
    }

    /// <summary>
    /// Gets all of the allowed operations for the /api/v1/Customer endpoints.
    /// </summary>
    /// <returns>A list of the allowed operations for the /api/v1/Customer endpoints.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     OPTIONS /api/v1/Customer
    /// </remarks>
    [HttpOptions]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns all of the options in the \"Allow\" response header.")]
    public ActionResult GetCustomerOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PATCH, DELETE, OPTIONS");
        return Ok();
    }

    /// <summary>
    /// Gets all of the allowed operations for the /api/v1/Customer/{id}/* endpoints.
    /// </summary>
    /// <returns>A list of the allowed operations for the /api/v1/Customer/{id}/* endpoints.</returns>
    /// <remarks>
    /// Sample request for /api/v1/Customer/{id}/RentBooks:
    /// 
    ///     OPTIONS /api/v1/Customer/3fa85f64-5717-4562-b3fc-2c963f66afa6/RentBooks
    /// 
    /// Sample request for /api/v1/Customer/{id}/ReturnBooks:
    /// 
    ///     OPTIONS /api/v1/Customer/3fa85f64-5717-4562-b3fc-2c963f66afa6/ReturnBooks
    /// </remarks>
    [HttpOptions("{id}/RentBooks")]
    [HttpOptions("{id}/ReturnBooks")]
    public async Task<ActionResult> GetCustomerRentAndReturnBooksOptionsAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<GetCustomerDto> getCustomerByIdResult = await _dispatcher.DispatchAsync(
            new GetCustomerByIdQuery(id),
            cancellationToken);
        if (!getCustomerByIdResult.IsSuccess)
        {
            return HandleError(getCustomerByIdResult);
        }
        Response.Headers.Append("Allow", "PATCH, OPTIONS");
        return Ok();
    }
}
