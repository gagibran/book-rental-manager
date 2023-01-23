namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand>
{
    private readonly IRepository<Customer> _customerRepository;

    public CreateCustomerCommandHandler(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result> HandleAsync(CreateCustomerCommand command, CancellationToken cancellationToken)
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