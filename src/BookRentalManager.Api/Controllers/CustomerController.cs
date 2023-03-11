using BookRentalManager.Application.Customers.Commands;
using BookRentalManager.Application.Customers.Queries;

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
            return HandleError(getAllCustomersResult);
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
            return HandleError(getCustomerByIdResult);
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
            return HandleError(createCustomerResult);
        }
        return CreatedAtAction(nameof(GetCustomerByIdAsync), new { Id = createCustomerResult.Value!.Id }, createCustomerResult.Value);
    }

    [HttpPatch("{id}")]
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

    [HttpPatch("{id}/rentBooks")]
    [HttpPatch("{id}/returnBooks")]
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

    [HttpDelete("{id}")]
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

    [HttpOptions("{id}/rentBooks")]
    [HttpOptions("{id}/returnBooks")]
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

    protected override ActionResult HandleError(Result result)
    {
        _baseControllerLogger.LogError(result.ErrorMessage);
        switch (result.ErrorType)
        {
            case "customerId":
            case "bookIds":
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.NotFound);
            case "jsonPatch":
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.BadRequest);
            default:
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.UnprocessableEntity);
        }
    }
}
