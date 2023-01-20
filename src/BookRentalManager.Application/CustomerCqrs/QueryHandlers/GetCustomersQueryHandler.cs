namespace BookRentalManager.Application.CustomerCqrs.QueryHandlers;

internal sealed class GetCustomersQueryHandler
    : IQueryHandler<GetCustomersQuery, IReadOnlyList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomersQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetCustomerDto>>> HandleAsync(
        GetCustomersQuery getCustomersQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Customer> customers = await _customerRepository.GetAllAsync(
            getCustomersQuery.PageIndex,
            getCustomersQuery.TotalItemsPerPage,
            new CustomersWithBooksSpecification(),
            cancellationToken);
        IEnumerable<GetCustomerDto> getCustomersDto = from customer in customers
                                                      select _getCustomerDtoMapper.Map(customer);
        return Result.Success<IReadOnlyList<GetCustomerDto>>(getCustomersDto.ToList());
    }
}
