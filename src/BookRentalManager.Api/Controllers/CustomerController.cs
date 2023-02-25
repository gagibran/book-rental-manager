using BookRentalManager.Application.Customers.Commands;
using BookRentalManager.Application.Customers.Queries;
using Microsoft.AspNetCore.JsonPatch;

namespace BookRentalManager.Api.Controllers;

[Route("api/[controller]")]
public sealed class CustomerController : ApiController
{
    public CustomerController(IDispatcher dispatcher, ILogger<CustomerController> customerControllerLogger)
        : base(dispatcher, customerControllerLogger)
    {
    }

    [HttpGet(Name = nameof(GetCustomersByQueryParametersAsync))]
    [HttpHead]
    public async Task<ActionResult<PaginatedList<GetCustomerDto>>> GetCustomersByQueryParametersAsync(
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
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
            _baseControllerLogger.LogError(getAllCustomersResult.ErrorMessage);
            return CustomHttpErrorResponse(
                getAllCustomersResult.ErrorType,
                getAllCustomersResult.ErrorMessage,
                HttpStatusCode.BadRequest);
        }
        CreatePagingMetadata(
            nameof(GetCustomersByQueryParametersAsync),
            queryParameters.SearchQuery,
            queryParameters.SortBy,
            getAllCustomersResult.Value!);
        return Ok(getAllCustomersResult.Value);
    }

    [HttpGet("{id}")]
    [HttpHead("{id}")]
    [ActionName(nameof(GetCustomerByIdAsync))]
    public async Task<ActionResult<GetCustomerDto>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<GetCustomerDto> getCustomerByIdResult = await _dispatcher.DispatchAsync<GetCustomerDto>(
            new GetCustomerByIdQuery(id),
            cancellationToken);
        if (!getCustomerByIdResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getCustomerByIdResult.ErrorMessage);
            return CustomHttpErrorResponse(getCustomerByIdResult.ErrorType, getCustomerByIdResult.ErrorMessage, HttpStatusCode.NotFound);
        }
        return Ok(getCustomerByIdResult.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCustomerAsync(CreateCustomerCommand createCustomerCommand, CancellationToken cancellationToken)
    {
        Result<CustomerCreatedDto> createCustomerResult = await _dispatcher.DispatchAsync<CustomerCreatedDto>(
            createCustomerCommand,
            cancellationToken);
        if (!createCustomerResult.IsSuccess)
        {
            _baseControllerLogger.LogError(createCustomerResult.ErrorMessage);
            return CustomHttpErrorResponse(
                createCustomerResult.ErrorType,
                createCustomerResult.ErrorMessage,
                HttpStatusCode.UnprocessableEntity);
        }
        return CreatedAtAction(nameof(GetCustomerByIdAsync), new { id = createCustomerResult.Value!.Id }, createCustomerResult.Value);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchCustomerNameAndPhoneNumberAsync(
        Guid id,
        JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto> patchCustomerNameAndPhoneNumberDtoPatchDocument,
        CancellationToken cancellationToken)
    {
        var patchCustomerNameAndPhoneNumberCommand = new PatchCustomerNameAndPhoneNumberCommand(id, patchCustomerNameAndPhoneNumberDtoPatchDocument);
        Result patchCustomerNameAndPhoneNumberResult = await _dispatcher.DispatchAsync(patchCustomerNameAndPhoneNumberCommand, cancellationToken);
        if (!patchCustomerNameAndPhoneNumberResult.IsSuccess && patchCustomerNameAndPhoneNumberResult.ErrorType.Equals("customerId"))
        {
            _baseControllerLogger.LogError(patchCustomerNameAndPhoneNumberResult.ErrorMessage);
            return CustomHttpErrorResponse(
                patchCustomerNameAndPhoneNumberResult.ErrorType,
                patchCustomerNameAndPhoneNumberResult.ErrorMessage,
                HttpStatusCode.NotFound);
        }
        else if (!patchCustomerNameAndPhoneNumberResult.IsSuccess && !patchCustomerNameAndPhoneNumberResult.ErrorType.Equals("customerId"))
        {
            _baseControllerLogger.LogError(patchCustomerNameAndPhoneNumberResult.ErrorMessage);
            return CustomHttpErrorResponse(
                patchCustomerNameAndPhoneNumberResult.ErrorType,
                patchCustomerNameAndPhoneNumberResult.ErrorMessage,
                HttpStatusCode.BadRequest);
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleteCustomerByIdCommand = new DeleteCustomerByIdCommand(id);
        Result deleteCustomerByIdResult = await _dispatcher.DispatchAsync(deleteCustomerByIdCommand, cancellationToken);
        if (!deleteCustomerByIdResult.IsSuccess && deleteCustomerByIdResult.ErrorType.Equals("customerId"))
        {
            _baseControllerLogger.LogError(deleteCustomerByIdResult.ErrorMessage);
            return CustomHttpErrorResponse(
                deleteCustomerByIdResult.ErrorType,
                deleteCustomerByIdResult.ErrorMessage,
                HttpStatusCode.NotFound);
        }
        else if (!deleteCustomerByIdResult.IsSuccess && !deleteCustomerByIdResult.ErrorType.Equals("customerId"))
        {
            _baseControllerLogger.LogError(deleteCustomerByIdResult.ErrorMessage);
            return CustomHttpErrorResponse(
                deleteCustomerByIdResult.ErrorType,
                deleteCustomerByIdResult.ErrorMessage,
                HttpStatusCode.UnprocessableEntity);
        }
        return NoContent();
    }

    [HttpOptions]
    public ActionResult GetCustomerOptions()
    {
        Response.Headers.Add("Allow", "GET, HEAD, POST, PATCH, DELETE, OPTIONS");
        return Ok();
    }
}
