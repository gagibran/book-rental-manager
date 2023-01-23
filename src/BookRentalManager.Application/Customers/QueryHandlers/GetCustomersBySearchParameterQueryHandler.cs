namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomersBySearchParameterQueryHandler
    : IQueryHandler<GetCustomersBySearchParameterQuery, IReadOnlyList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomersBySearchParameterQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetCustomerDto>>> HandleAsync(
        GetCustomersBySearchParameterQuery getCustomersBySearchParameterQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Customer> customers = await _customerRepository.GetAllBySpecificationAsync(
            getCustomersBySearchParameterQuery.PageIndex,
            getCustomersBySearchParameterQuery.TotalItemsPerPage,
            new CustomersBySearchParameterSpecification(getCustomersBySearchParameterQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetCustomerDto> getCustomerDtos = (from customer in customers
                                                         select _getCustomerDtoMapper.Map(customer)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetCustomerDto>>(getCustomerDtos);
    }
}
