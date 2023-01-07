using BookRentalManager.Application.CustomerCqrs.Queries;
using BookRentalManager.Application.Dtos;
using BookRentalManager.Application.Interfaces;
using BookRentalManager.Domain.Common;

namespace BookRentalManager.Api.Controllers;

public sealed class CustomerController : BaseController
{
    public CustomerController(IDispatcher dispatcher, ILogger<CustomerController> customerControllerLogger)
        : base(dispatcher, customerControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetCustomerDto>>> GetCustomersAsync(
        int pageIndex = 1,
        int totalItemsPerPage = 50
    )
    {
        Result<IReadOnlyList<GetCustomerDto>> getAllCustomersResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetCustomerDto>>(
            new GetCustomersQuery(pageIndex, totalItemsPerPage),
            default
        );
        if (!getAllCustomersResult.IsSuccess)
        {
            _baseControllerLogger.Log(LogLevel.Error, getAllCustomersResult.ErrorMessage);
            return NotFound(getAllCustomersResult.ErrorMessage);
        }
        return Ok(getAllCustomersResult.Value);
    }

    // [HttpGet("{id}")]
    // public async Task<ActionResult<GetCustomerDto>> GetCustomerByIdAsync(Guid id)
    // {
    //     Result<GetCustomerDto> getCustomerByIdResult = await _dispatcher.DispatchAsync<GetCustomerDto>(
    //         new GetC
    //     );
    // }
}
