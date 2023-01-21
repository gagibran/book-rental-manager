namespace BookRentalManager.Application.CustomerCqrs.QueryHandlers;

internal sealed class GetCustomersWithBooksAndSearchParamQueryHandler
    : IQueryHandler<GetCustomersWithBooksAndSearchParamQuery, IReadOnlyList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomersWithBooksAndSearchParamQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetCustomerDto>>> HandleAsync(
        GetCustomersWithBooksAndSearchParamQuery getCustomersWithBooksAndSearchParamQuery,
        CancellationToken cancellationToken)
    {
        var customersWithBooksAndSearchParamSpecification = new CustomersWithBooksAndSearchParamSpecification(
        getCustomersWithBooksAndSearchParamQuery.SearchParameter);
        IReadOnlyList<Customer> customers = await _customerRepository.GetAllBySpecificationAsync(
            getCustomersWithBooksAndSearchParamQuery.PageIndex,
            getCustomersWithBooksAndSearchParamQuery.TotalItemsPerPage,
            customersWithBooksAndSearchParamSpecification,
            cancellationToken);
        IEnumerable<GetCustomerDto> getCustomersDto = from customer in customers
                                                      select _getCustomerDtoMapper.Map(customer);
        return Result.Success<IReadOnlyList<GetCustomerDto>>(getCustomersDto.ToList());
    }
}
