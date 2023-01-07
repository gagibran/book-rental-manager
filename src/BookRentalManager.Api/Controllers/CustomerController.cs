using BookRentalManager.Application.CustomerCqrs.Queries;
using BookRentalManager.Application.Dtos;
using BookRentalManager.Application.Interfaces;
using BookRentalManager.Domain.Common;

namespace BookRentalManager.Api.Controllers;

public sealed class CustomerController : BaseController
{
    private readonly IDispatcher _dispatcher;
    private readonly ILogger<CustomerController> _customerControllerLogger;

    public CustomerController(
        IDispatcher dispatcher,
        ILogger<CustomerController> customerControllerLogger
    )
    {
        _customerControllerLogger = customerControllerLogger;
        _dispatcher = dispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetCustomerDto>>> GetCustomersAsync()
    {
        Result<IReadOnlyList<GetCustomerDto>> getAllCustomersResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetCustomerDto>>(
            new GetAllCustomersQuery(),
            default
        );
        if (!getAllCustomersResult.IsSuccess)
        {
            return NotFound(getAllCustomersResult.ErrorMessage);
        }
        return Ok(getAllCustomersResult.Value);
    }
}
