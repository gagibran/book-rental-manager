namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class DeleteCustomerByIdCommandHandler(IRepository<Customer> customerRepository) : IRequestHandler<DeleteCustomerByIdCommand>
{
    public async Task<Result> HandleAsync(DeleteCustomerByIdCommand deleteCustomerByIdCommand, CancellationToken cancellationToken)
    {
        var customerByIdWithBooksSpecification = new CustomerByIdWithBooksSpecification(deleteCustomerByIdCommand.Id);
        Customer? customer = await customerRepository.GetFirstOrDefaultBySpecificationAsync(
            customerByIdWithBooksSpecification,
            cancellationToken);
        if (customer is null)
        {
            return Result.Fail(RequestErrors.IdNotFoundError, $"No customer with the ID of '{deleteCustomerByIdCommand.Id}' was found.");
        }
        if (customer.Books.Any())
        {
            return Result.Fail("customerBooks", "This customer has rented books. Return them before deleting.");
        }
        await customerRepository.DeleteAsync(customer, cancellationToken);
        return Result.Success();
    }
}
