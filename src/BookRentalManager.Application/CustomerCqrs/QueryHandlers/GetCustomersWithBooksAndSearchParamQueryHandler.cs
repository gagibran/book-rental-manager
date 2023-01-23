namespace BookRentalManager.Application.CustomerCqrs.QueryHandlers;

internal sealed class GetCustomersWithSearchParamQueryHandler
    : IQueryHandler<GetCustomersWithSearchParamQuery, IReadOnlyList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomersWithSearchParamQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetCustomerDto>>> HandleAsync(
        GetCustomersWithSearchParamQuery getCustomersWithSearchParamQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Customer> customers = await _customerRepository.GetAllBySpecificationAsync(
            getCustomersWithSearchParamQuery.PageIndex,
            getCustomersWithSearchParamQuery.TotalItemsPerPage,
            new CustomersWithSearchParamSpecification(getCustomersWithSearchParamQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetCustomerDto> getCustomerDtos = (from customer in customers
                                                         select _getCustomerDtoMapper.Map(customer)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetCustomerDto>>(getCustomerDtos);
    }
}
