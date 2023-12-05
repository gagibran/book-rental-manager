namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomersByQueryParametersQueryHandler(
    IRepository<Customer> customerRepository,
    ISortParametersMapper sortParametersMapper)
    : IRequestHandler<GetCustomersByQueryParametersQuery, PaginatedList<GetCustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository = customerRepository;
    private readonly ISortParametersMapper _sortParametersMapper = sortParametersMapper;

    public async Task<Result<PaginatedList<GetCustomerDto>>> HandleAsync(
        GetCustomersByQueryParametersQuery getCustomersByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        Result<string> convertedSortParametersResult = _sortParametersMapper.MapCustomerSortParameters(
            getCustomersByQueryParametersQuery.SortParameters);
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
        List<GetCustomerDto> getCustomerDtos = customers
            .Select(customer => new GetCustomerDto(customer))
            .ToList();
        var paginatedGetCustomerDtos = new PaginatedList<GetCustomerDto>(
            getCustomerDtos,
            customers.TotalAmountOfItems,
            customers.TotalAmountOfPages,
            customers.PageIndex,
            customers.PageSize);
        return Result.Success(paginatedGetCustomerDtos);
    }
}
