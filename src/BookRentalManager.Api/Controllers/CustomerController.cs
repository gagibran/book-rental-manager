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

    [HttpGet]
    public async Task<ActionResult<PaginatedList<GetCustomerDto>>> GetCustomersByQueryParametersAsync(
        [FromQuery] GetAllQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var getCustomersByQueryParametersQuery = new GetCustomersByQueryParametersQuery(
            queryParameters.PageIndex,
            queryParameters.PageSize,
            queryParameters.SearchQuery);
        Result<PaginatedList<GetCustomerDto>> getAllCustomersResult = await _dispatcher.DispatchAsync<PaginatedList<GetCustomerDto>>(
            getCustomersByQueryParametersQuery,
            cancellationToken);
        CreatePagingMetadata(nameof(GetCustomersByQueryParametersAsync), queryParameters.SearchQuery, getAllCustomersResult.Value!);
        return Ok(getAllCustomersResult.Value);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetCustomerByIdAsync))]
    public async Task<ActionResult<GetCustomerDto>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<GetCustomerDto> getCustomerByIdResult = await _dispatcher.DispatchAsync<GetCustomerDto>(
            new GetCustomerByIdQuery(id),
            cancellationToken);
        if (!getCustomerByIdResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getCustomerByIdResult.ErrorMessage);
            return CustomNotFound<GetCustomerDto>(getCustomerByIdResult.ErrorType, getCustomerByIdResult.ErrorMessage);
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
            return CustomUnprocessableEntity(createCustomerResult.ErrorType, createCustomerResult.ErrorMessage);
        }
        return CreatedAtAction("Test", new { id = createCustomerResult.Value!.Id }, createCustomerResult.Value);
    }
}
