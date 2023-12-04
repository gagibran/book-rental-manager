namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomerByIdQueryHandler(
    IRepository<Customer> customerRepository,
    IMapper<Customer, GetCustomerDto> customerToGetCustomerDtoMapper)
    : IRequestHandler<GetCustomerByIdQuery, GetCustomerDto>
{
    private readonly IRepository<Customer> _customerRepository = customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _customerToGetCustomerDtoMapper = customerToGetCustomerDtoMapper;

    public async Task<Result<GetCustomerDto>> HandleAsync(GetCustomerByIdQuery getCustomerByIdQuery, CancellationToken cancellationToken)
    {
        var customerByIdWithBooksSpecification = new CustomerByIdWithBooksSpecification(getCustomerByIdQuery.Id);
        var customer = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(customerByIdWithBooksSpecification, cancellationToken);
        if (customer is null)
        {
            return Result.Fail<GetCustomerDto>("customerId", $"No customer with the ID of '{getCustomerByIdQuery.Id}' was found.");
        }
        return Result.Success(_customerToGetCustomerDtoMapper.Map(customer));
    }
}
