namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomersByQueryParameterQueryHandler
    : IQueryHandler<GetCustomersByQueryParameterQuery, IReadOnlyList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomersByQueryParameterQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper
    )
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetCustomerDto>>> HandleAsync(
        GetCustomersByQueryParameterQuery getCustomersByQueryParameterQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Customer> customers = await _customerRepository.GetAllBySpecificationAsync(
            getCustomersByQueryParameterQuery.PageIndex,
            getCustomersByQueryParameterQuery.TotalItemsPerPage,
            new CustomersBySearchParameterSpecification(getCustomersByQueryParameterQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetCustomerDto> getCustomerDtos = (from customer in customers
                                                         select _getCustomerDtoMapper.Map(customer)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetCustomerDto>>(getCustomerDtos);
    }
}
