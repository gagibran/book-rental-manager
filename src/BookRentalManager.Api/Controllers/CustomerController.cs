using BookRentalManager.Application.Customers.Commands;
using BookRentalManager.Application.Customers.Queries;

namespace BookRentalManager.Api.Controllers;

[Route("api/[controller]")]
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
        [FromQuery(Name = "search")] string searchParameter = "")
    {
        var getCustomersBySearchParameterQuery = new GetCustomersBySearchParameterQuery(
            pageIndex,
            totalItemsPerPage,
            searchParameter);
        Result<IReadOnlyList<GetCustomerDto>> getAllCustomersResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetCustomerDto>>(
            getCustomersBySearchParameterQuery,
            cancellationToken);
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
            return NotFound(getCustomerByIdResult.ErrorMessage);
        }
        return Ok(getCustomerByIdResult.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCustomerAsync(CreateCustomerDto createCustomerDto, CancellationToken cancellationToken)
    {
        Result<FullName> fullNameResult = FullName.Create(createCustomerDto.FirstName, createCustomerDto.LastName);
        Result<Email> emailResult = Email.Create(createCustomerDto.Email);
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(createCustomerDto.AreaCode, createCustomerDto.PhoneNumber);
        Result combinedResults = Result.Combine(
            fullNameResult,
            emailResult,
            phoneNumberResult);
        if (!combinedResults.IsSuccess)
        {
            _baseControllerLogger.LogError(combinedResults.ErrorMessage);
            return BadRequest(combinedResults.ErrorMessage);
        }
        var newCustomer = new Customer(fullNameResult.Value!, emailResult.Value!, phoneNumberResult.Value!);
        Result createCustomerResult = await _dispatcher.DispatchAsync(new CreateCustomerCommand(newCustomer), cancellationToken);
        if (!createCustomerResult.IsSuccess)
        {
            _baseControllerLogger.LogError(createCustomerResult.ErrorMessage);
            return BadRequest(createCustomerResult.ErrorMessage);
        }
        var customerCreatedDto = new CustomerCreatedDto(
            newCustomer.Id,
            newCustomer.FullName.CompleteName,
            newCustomer.Email.EmailAddress,
            newCustomer.PhoneNumber.CompletePhoneNumber,
            newCustomer.CustomerStatus.CustomerType.ToString(),
            newCustomer.CustomerPoints);
        return CreatedAtAction(nameof(GetCustomerByIdAsync), new { id = newCustomer.Id }, customerCreatedDto);
    }
}
