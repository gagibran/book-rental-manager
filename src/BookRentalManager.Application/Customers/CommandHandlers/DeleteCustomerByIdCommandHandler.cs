namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class DeleteCustomerByIdCommandHandler : ICommandHandler<DeleteCustomerByIdCommand>
{
    private readonly IRepository<Customer> _customerRepository;

    public DeleteCustomerByIdCommandHandler(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result> HandleAsync(DeleteCustomerByIdCommand deleteCustomerByIdCommand, CancellationToken cancellationToken)
    {
        var customerByIdWithBooksSpecification = new CustomerByIdWithBooksSpecification(deleteCustomerByIdCommand.Id);
        Customer? customer = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(
            customerByIdWithBooksSpecification,
            cancellationToken);
        if (customer is null)
        {
            return Result.Fail("customerId", $"No customer with the ID of '{deleteCustomerByIdCommand.Id}' was found.");
        }
        if (customer.Books.Any())
        {
            return Result.Fail("customerBooks", "This customer has rented books. Return them before deleting.");
        }
        await _customerRepository.DeleteAsync(customer, cancellationToken);
        return Result.Success();
    }
}
