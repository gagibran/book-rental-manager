using BookRentalManager.Application.CustomerCqrs.Queries;

namespace BookRentalManager.Api.Controllers;

public sealed class CustomerController : BaseController
{
    public CustomerController(IDispatcher dispatcher, ILogger<CustomerController> customerControllerLogger)
        : base(dispatcher, customerControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetCustomerDto>>> GetCustomersAsync(
        CancellationToken cancellationToken,
        int pageIndex = 1,
        int totalItemsPerPage = 50,
        [FromQuery(Name = "search")] string queryParameter = ""
    )
    {
        IQuery<IReadOnlyList<GetCustomerDto>> getCustomersQuery = new GetCustomersQuery(pageIndex, totalItemsPerPage);
        if (!string.IsNullOrWhiteSpace(queryParameter))
        {
            getCustomersQuery = new GetCustomersWithQueryParameterQuery(
                pageIndex,
                totalItemsPerPage,
                queryParameter
            );
        }
        Result<IReadOnlyList<GetCustomerDto>> getAllCustomersResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetCustomerDto>>(
            getCustomersQuery,
            cancellationToken
        );
        if (!getAllCustomersResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getAllCustomersResult.ErrorMessage);
            return NotFound(getAllCustomersResult.ErrorMessage);
        }
        return Ok(getAllCustomersResult.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCustomerDto>> GetCustomerByIdAsync(CancellationToken cancellationToken, Guid id)
    {
        Result<GetCustomerDto> getCustomerByIdResult = await _dispatcher.DispatchAsync<GetCustomerDto>(
            new GetCustomerByIdQuery(id),
            cancellationToken
        );
        if (!getCustomerByIdResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getCustomerByIdResult.ErrorMessage);
            return NotFound(getCustomerByIdResult.ErrorMessage);
        }
        return Ok(getCustomerByIdResult.Value);
    }
}
