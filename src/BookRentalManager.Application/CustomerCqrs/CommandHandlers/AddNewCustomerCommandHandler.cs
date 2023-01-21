namespace BookRentalManager.Application.CustomerCqrs.CommandHandlers;

internal sealed class AddNewCustomerCommandHandler : ICommandHandler<AddNewCustomerCommand>
{
    private readonly IRepository<Customer> _customerRepository;

    public AddNewCustomerCommandHandler(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result> HandleAsync(AddNewCustomerCommand command, CancellationToken cancellationToken)
    {
        Customer? customerWithEmail = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(
            new CustomerWithBooksByEmailSpecification(command.Customer.Email.EmailAddress));
        if (customerWithEmail is not null)
        {
            return Result.Fail($"A customer with the email '{customerWithEmail.Email.EmailAddress}' already exists.");
        }
        await _customerRepository.CreateAsync(command.Customer, cancellationToken);
        return Result.Success();
    }
}
