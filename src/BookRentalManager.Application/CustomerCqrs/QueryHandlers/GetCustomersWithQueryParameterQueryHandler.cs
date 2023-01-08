namespace BookRentalManager.Application.CustomerCqrs.QueryHandlers;

internal sealed class GetCustomersWithQueryParameterQueryHandler
    : IQueryHandler<GetCustomersWithQueryParameterQuery, IReadOnlyList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomersWithQueryParameterQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetCustomerDto>>> HandleAsync(
        GetCustomersWithQueryParameterQuery getCustomersWithQueryParameterQuery,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Customer> customers = await _customerRepository.GetAllAsync(
            getCustomersWithQueryParameterQuery.PageIndex,
            getCustomersWithQueryParameterQuery.TotalItemsPerPage,
            new CustomerWithQueryParameterSpecification(getCustomersWithQueryParameterQuery.QueryParameter),
            cancellationToken
        );
        if (!customers.Any())
        {
            return Result.Fail<IReadOnlyList<GetCustomerDto>>(
                $"There are currently no customers containing the value '{getCustomersWithQueryParameterQuery.QueryParameter}' registered."
            );
        }
        IEnumerable<GetCustomerDto> getCustomersDto = from customer in customers
                                                      select _getCustomerDtoMapper.Map(customer);
        return Result.Success<IReadOnlyList<GetCustomerDto>>(getCustomersDto.ToList());
    }
}
