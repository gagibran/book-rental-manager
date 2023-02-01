namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomersByQueryParametersQueryHandler
    : IQueryHandler<GetCustomersByQueryParametersQuery, PaginatedList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _getCustomerDtoMapper;

    public GetCustomersByQueryParametersQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> getCustomerDtoMapper)
    {
        _customerRepository = customerRepository;
        _getCustomerDtoMapper = getCustomerDtoMapper;
    }

    public async Task<Result<PaginatedList<GetCustomerDto>>> HandleAsync(
        GetCustomersByQueryParametersQuery getCustomersByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        PaginatedList<Customer> customers = await _customerRepository.GetAllBySpecificationAsync(
            getCustomersByQueryParametersQuery.PageIndex,
            getCustomersByQueryParametersQuery.TotalAmountOfItemsPerPage,
            new CustomersBySearchParameterSpecification(getCustomersByQueryParametersQuery.SearchParameter),
            cancellationToken);
        List<GetCustomerDto> getCustomerDtos = (from customer in customers
                                                select _getCustomerDtoMapper.Map(customer)).ToList();
        var paginatedGetCustomerDtos = new PaginatedList<GetCustomerDto>(
            getCustomerDtos,
            getCustomersByQueryParametersQuery.PageIndex,
            getCustomersByQueryParametersQuery.TotalAmountOfItemsPerPage);
        return Result.Success<PaginatedList<GetCustomerDto>>(paginatedGetCustomerDtos);
    }
}
