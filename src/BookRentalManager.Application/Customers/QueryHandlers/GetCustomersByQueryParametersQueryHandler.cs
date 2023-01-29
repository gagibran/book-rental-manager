namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomersByQueryParametersQueryHandler
    : IQueryHandler<GetCustomersByQueryParametersQuery, IReadOnlyList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomersByQueryParametersQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetCustomerDto>>> HandleAsync(
        GetCustomersByQueryParametersQuery getCustomersByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Customer> customers = await _customerRepository.GetAllBySpecificationAsync(
            getCustomersByQueryParametersQuery.PageIndex,
            getCustomersByQueryParametersQuery.TotalItemsPerPage,
            new CustomersBySearchParameterSpecification(getCustomersByQueryParametersQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetCustomerDto> getCustomerDtos = (from customer in customers
                                                         select _getCustomerDtoMapper.Map(customer)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetCustomerDto>>(getCustomerDtos);
    }
}
