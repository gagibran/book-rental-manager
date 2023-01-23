namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class CreateNewCustomerCommandHandler : ICommandHandler<CreateNewCustomerCommand>
{
    private readonly IRepository<Customer> _customerRepository;

    public CreateNewCustomerCommandHandler(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result> HandleAsync(CreateNewCustomerCommand command, CancellationToken cancellationToken)
    {
        Customer? customerWithEmail = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(
            new CustomerByEmailSpecification(command.Customer.Email.EmailAddress));
        if (customerWithEmail is not null)
        {
            return Result.Fail($"A customer with the email '{customerWithEmail.Email.EmailAddress}' already exists.");
        }
        await _customerRepository.CreateAsync(command.Customer, cancellationToken);
        return Result.Success();
    }
}
