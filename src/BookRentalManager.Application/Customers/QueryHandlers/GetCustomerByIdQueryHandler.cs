namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, GetCustomerDto>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _customerToGetCustomerDtoMapper;

    public GetCustomerByIdQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> customerToGetCustomerDtoMapper)
    {
        _customerRepository = customerRepository;
        _customerToGetCustomerDtoMapper = customerToGetCustomerDtoMapper;
    }

    public async Task<Result<GetCustomerDto>> HandleAsync(GetCustomerByIdQuery getCustomerByIdQuery, CancellationToken cancellationToken)
    {
        var customerByIdSpecification = new CustomerByIdSpecification(getCustomerByIdQuery.Id);
        var customer = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(customerByIdSpecification, cancellationToken);
        if (customer is null)
        {
            return Result.Fail<GetCustomerDto>("customerId", $"No customer with the ID of '{getCustomerByIdQuery.Id}' was found.");
        }
        return Result.Success<GetCustomerDto>(_customerToGetCustomerDtoMapper.Map(customer));
    }
}
