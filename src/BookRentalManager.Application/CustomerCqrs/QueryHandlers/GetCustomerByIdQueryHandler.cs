namespace BookRentalManager.Application.CustomerCqrs.QueryHandlers;

internal sealed class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, GetCustomerDto>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomerByIdQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<GetCustomerDto>> HandleAsync(
        GetCustomerByIdQuery getCustomerByIdQuery,
        CancellationToken cancellationToken
    )
    {
        var customer = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(
            new CustomerWithBooksByIdSpecification(getCustomerByIdQuery.Id),
            cancellationToken);
        if (customer is null)
        {
            return Result.Fail<GetCustomerDto>($"No customer with the ID of '{getCustomerByIdQuery.Id} was found.");
        }
        return Result.Success<GetCustomerDto>(_getCustomerDtoMapper.Map(customer));
    }
}
