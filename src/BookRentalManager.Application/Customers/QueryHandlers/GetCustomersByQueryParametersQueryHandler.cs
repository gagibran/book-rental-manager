namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomersByQueryParametersQueryHandler
    : IQueryHandler<GetCustomersByQueryParametersQuery, PaginatedList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _customerToGetCustomerDtoMapper;

    public GetCustomersByQueryParametersQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> customerToGetCustomerDtoMapper)
    {
        _customerRepository = customerRepository;
        _customerToGetCustomerDtoMapper = customerToGetCustomerDtoMapper;
    }

    public async Task<Result<PaginatedList<GetCustomerDto>>> HandleAsync(
        GetCustomersByQueryParametersQuery getCustomersByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        PaginatedList<Customer> customers = await _customerRepository.GetAllBySpecificationAsync(
            getCustomersByQueryParametersQuery.PageIndex,
            getCustomersByQueryParametersQuery.PageSize,
            new CustomersBySearchParameterSpecification(getCustomersByQueryParametersQuery.SearchParameter),
            cancellationToken);
        List<GetCustomerDto> getCustomerDtos = (from customer in customers
                                                select _customerToGetCustomerDtoMapper.Map(customer)).ToList();
        var paginatedGetCustomerDtos = new PaginatedList<GetCustomerDto>(
            getCustomerDtos,
            customers.TotalAmountOfItems,
            customers.TotalAmountOfPages,
            customers.PageIndex,
            customers.PageSize);
        return Result.Success<PaginatedList<GetCustomerDto>>(paginatedGetCustomerDtos);
    }
}
