namespace BookRentalManager.Application.Customers.QueryHandlers;

internal sealed class GetCustomerByIdQueryHandler(IRepository<Customer> customerRepository)
    : IRequestHandler<GetCustomerByIdQuery, GetCustomerDto>
{
    public async Task<Result<GetCustomerDto>> HandleAsync(GetCustomerByIdQuery getCustomerByIdQuery, CancellationToken cancellationToken)
    {
        var customerByIdWithBooksSpecification = new CustomerByIdWithBooksSpecification(getCustomerByIdQuery.Id);
        var customer = await customerRepository.GetFirstOrDefaultBySpecificationAsync(customerByIdWithBooksSpecification, cancellationToken);
        if (customer is null)
        {
            return Result.Fail<GetCustomerDto>(RequestErrors.IdNotFoundError, $"No customer with the ID of '{getCustomerByIdQuery.Id}' was found.");
        }
        return Result.Success(new GetCustomerDto(customer));
    }
}
