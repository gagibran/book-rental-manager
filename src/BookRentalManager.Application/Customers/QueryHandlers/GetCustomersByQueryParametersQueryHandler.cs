namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomersByQueryParametersQueryHandler
    : IQueryHandler<GetCustomersByQueryParametersQuery, PaginatedList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _customerToGetCustomerDtoMapper;
    private readonly IMapper<CustomerSortParameters, Result<string>> _customerSortParametersMapper;

    public GetCustomersByQueryParametersQueryHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, GetCustomerDto> customerToGetCustomerDtoMapper,
        IMapper<CustomerSortParameters, Result<string>> customerSortParametersMapper)
    {
        _customerRepository = customerRepository;
        _customerToGetCustomerDtoMapper = customerToGetCustomerDtoMapper;
        _customerSortParametersMapper = customerSortParametersMapper;
    }

    public async Task<Result<PaginatedList<GetCustomerDto>>> HandleAsync(
        GetCustomersByQueryParametersQuery getCustomersByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        Result<string> convertedSortParametersResult = _customerSortParametersMapper.Map(
            new CustomerSortParameters(getCustomersByQueryParametersQuery.SortParameters));
        if (!convertedSortParametersResult.IsSuccess)
        {
            return Result.Fail<PaginatedList<GetCustomerDto>>(
                convertedSortParametersResult.ErrorType,
                convertedSortParametersResult.ErrorMessage);
        }
        var customersBySearchParameterSpecification = new CustomersBySearchParameterSpecification(
            getCustomersByQueryParametersQuery.SearchParameter,
            convertedSortParametersResult.Value!);
        PaginatedList<Customer> customers = await _customerRepository.GetAllBySpecificationAsync(
            getCustomersByQueryParametersQuery.PageIndex,
            getCustomersByQueryParametersQuery.PageSize,
            customersBySearchParameterSpecification,
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
