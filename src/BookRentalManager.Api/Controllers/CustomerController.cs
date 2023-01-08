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
        string email = ""
    )
    {
        Result<IReadOnlyList<GetCustomerDto>> getAllCustomersResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetCustomerDto>>(
            new GetCustomersQuery(pageIndex, totalItemsPerPage, email),
            cancellationToken
        );
        if (!getAllCustomersResult.IsSuccess)
        {
            _baseControllerLogger.Log(LogLevel.Error, getAllCustomersResult.ErrorMessage);
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
            _baseControllerLogger.Log(LogLevel.Error, getCustomerByIdResult.ErrorMessage);
            return NotFound(getCustomerByIdResult.ErrorMessage);
        }
        return Ok(getCustomerByIdResult.Value);
    }
}
