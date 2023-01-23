namespace BookRentalManager.Application.CustomerCqrs.QueryHandlers;

internal sealed class GetCustomerWithBooksByIdQueryHandler : IQueryHandler<GetCustomerWithBooksByIdQuery, GetCustomerDto>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomerWithBooksByIdQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<GetCustomerDto>> HandleAsync(
        GetCustomerWithBooksByIdQuery getCustomerWithBooksByIdQuery,
        CancellationToken cancellationToken
    )
    {
        var customer = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(
            new CustomerByIdSpecification(getCustomerWithBooksByIdQuery.Id),
            cancellationToken);
        if (customer is null)
        {
            return Result.Fail<GetCustomerDto>($"No customer with the ID of '{getCustomerWithBooksByIdQuery.Id} was found.");
        }
        return Result.Success<GetCustomerDto>(_getCustomerDtoMapper.Map(customer));
    }
}
