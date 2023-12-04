namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomersByQueryParametersQueryHandler(
    IRepository<Customer> customerRepository,
    IMapper<Customer, GetCustomerDto> customerToGetCustomerDtoMapper,
    IMapper<CustomerSortParameters, Result<string>> customerSortParametersMapper)
    : IRequestHandler<GetCustomersByQueryParametersQuery, PaginatedList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository = customerRepository;
    private readonly IMapper<Customer, GetCustomerDto> _customerToGetCustomerDtoMapper = customerToGetCustomerDtoMapper;
    private readonly IMapper<CustomerSortParameters, Result<string>> _customerSortParametersMapper = customerSortParametersMapper;

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
        var customersBySearchParameterWithBooksSpecification = new CustomersBySearchParameterWithBooksSpecification(
            getCustomersByQueryParametersQuery.SearchParameter,
            convertedSortParametersResult.Value!);
        PaginatedList<Customer> customers = await _customerRepository.GetAllBySpecificationAsync(
            getCustomersByQueryParametersQuery.PageIndex,
            getCustomersByQueryParametersQuery.PageSize,
            customersBySearchParameterWithBooksSpecification,
            cancellationToken);
        List<GetCustomerDto> getCustomerDtos = (from customer in customers
                                                select _customerToGetCustomerDtoMapper.Map(customer)).ToList();
        var paginatedGetCustomerDtos = new PaginatedList<GetCustomerDto>(
            getCustomerDtos,
            customers.TotalAmountOfItems,
            customers.TotalAmountOfPages,
            customers.PageIndex,
            customers.PageSize);
        return Result.Success(paginatedGetCustomerDtos);
    }
}
