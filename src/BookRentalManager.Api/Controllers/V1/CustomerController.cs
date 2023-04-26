using BookRentalManager.Application.Customers.Commands;
using BookRentalManager.Application.Customers.Queries;

namespace BookRentalManager.Api.Controllers.V1;

[ApiVersion("1.0")]
public sealed class CustomerController : ApiController
{
    private readonly IDispatcher _dispatcher;
    private readonly List<AllowedRestMethodsDto> _allowedRestMethodDtos;

    public CustomerController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _allowedRestMethodDtos = new List<AllowedRestMethodsDto>
        {
            new AllowedRestMethodsDto(nameof(GetCustomerByIdAsync), "GET", "self"),
            new AllowedRestMethodsDto(nameof(PatchCustomerNameAndPhoneNumberByIdAsync), "PATCH", "patch_customer"),
            new AllowedRestMethodsDto("RentBooks", "PATCH", "rent_books"),
            new AllowedRestMethodsDto("ReturnBooks", "PATCH", "return_books"),
            new AllowedRestMethodsDto(nameof(DeleteCustomerByIdAsync), "DELETE", "delete_customer")
        };
    }

    [HttpGet(Name = nameof(GetCustomersByQueryParametersAsync))]
    [HttpHead]
    public async Task<ActionResult<PaginatedList<GetCustomerDto>>> GetCustomersByQueryParametersAsync(
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        [FromHeader(Name = "Accept")] string? mediaType,
        CancellationToken cancellationToken)
    {
        if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? mediaTypeHeaderValue))
        {
            return CustomHttpErrorResponse(MediaTypeConstants.MediaTypeErrorType, MediaTypeConstants.MediaTypeErrorMessage, HttpStatusCode.BadRequest);
        }
        var getCustomersByQueryParametersQuery = new GetCustomersByQueryParametersQuery(
            queryParameters.PageIndex,
            queryParameters.PageSize,
            queryParameters.SearchQuery,
            queryParameters.SortBy);
        Result<PaginatedList<GetCustomerDto>> getAllCustomersResult = await _dispatcher.DispatchAsync<PaginatedList<GetCustomerDto>>(
            getCustomersByQueryParametersQuery,
            cancellationToken);
        if (!getAllCustomersResult.IsSuccess)
        {
            return HandleError(getAllCustomersResult);
        }
        CreatePaginationMetadata(nameof(GetCustomersByQueryParametersAsync), getAllCustomersResult.Value!);
        if (mediaTypeHeaderValue.MediaType.Equals(MediaTypeConstants.BookRentalManagerHateoasMediaType))
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

    [HttpGet("{id}", Name = nameof(GetCustomerByIdAsync))]
    [HttpHead("{id}")]
    [ActionName(nameof(GetCustomerByIdAsync))]
    public async Task<ActionResult<GetCustomerDto>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<GetCustomerDto> getCustomerByIdResult = await _dispatcher.DispatchAsync<GetCustomerDto>(
            new GetCustomerByIdQuery(id),
            cancellationToken);
        if (!getCustomerByIdResult.IsSuccess)
        {
            return HandleError(getCustomerByIdResult);
        }
        return Ok(AddHateoasLinks(_allowedRestMethodDtos, getCustomerByIdResult.Value!));
    }

    [HttpPost]
    public async Task<ActionResult> CreateCustomerAsync(CreateCustomerCommand createCustomerCommand, CancellationToken cancellationToken)
    {
        Result<CustomerCreatedDto> createCustomerResult = await _dispatcher.DispatchAsync<CustomerCreatedDto>(
            createCustomerCommand,
            cancellationToken);
        if (!createCustomerResult.IsSuccess)
        {
            return HandleError(createCustomerResult);
        }
        return CreatedAtAction(
            nameof(GetCustomerByIdAsync),
            new { Id = createCustomerResult.Value!.Id },
            AddHateoasLinks(_allowedRestMethodDtos, createCustomerResult.Value));
    }

    [HttpPatch("{id}", Name = nameof(PatchCustomerNameAndPhoneNumberByIdAsync))]
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

    [HttpPatch("{id}/RentBooks", Name = "RentBooks")]
    [HttpPatch("{id}/ReturnBooks", Name = "ReturnBooks")]
    public async Task<ActionResult> ChangeCustomerBooksByBookIds(
        Guid id,
        JsonPatchDocument<ChangeCustomerBooksByBookIdsDto> changeCustomerBooksByBookIdsDtoPatchDocument,
        CancellationToken cancellationToken)
    {
        var isReturn = Request.Path.Value!.Contains("return");
        var changeCustomerBooksByBookIdsCommand = new ChangeCustomerBooksByBookIdsCommand(
            id,
            changeCustomerBooksByBookIdsDtoPatchDocument,
            isReturn);
        Result returnBookByBookIdResult = await _dispatcher.DispatchAsync(changeCustomerBooksByBookIdsCommand, cancellationToken);
        if (!returnBookByBookIdResult.IsSuccess)
        {
            return HandleError(returnBookByBookIdResult);
        }
        return NoContent();
    }

    [HttpDelete("{id}", Name = nameof(DeleteCustomerByIdAsync))]
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

    [HttpOptions]
    public ActionResult GetCustomerOptions()
    {
        Response.Headers.Add("Allow", "GET, HEAD, POST, PATCH, DELETE, OPTIONS");
        return Ok();
    }

    [HttpOptions("{id}/RentBooks")]
    [HttpOptions("{id}/ReturnBooks")]
    public async Task<ActionResult> GetCustomerRentAndReturnBooksOptionsAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<GetCustomerDto> getCustomerByIdResult = await _dispatcher.DispatchAsync<GetCustomerDto>(
            new GetCustomerByIdQuery(id),
            cancellationToken);
        if (!getCustomerByIdResult.IsSuccess)
        {
            return HandleError(getCustomerByIdResult);
        }
        Response.Headers.Add("Allow", "PATCH, OPTIONS");
        return Ok();
    }
}
