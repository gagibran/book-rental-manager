using BookRentalManager.Domain.Specifications;

namespace BookRentalManager.Application.CustomerCqrs.CommandHandlers;

internal sealed class AddNewCustomerCommandHandler : ICommandHandler<AddNewCustomerCommand>
{
    private readonly IRepository<Customer> _customerRepository;

    public AddNewCustomerCommandHandler(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result> HandleAsync(
        AddNewCustomerCommand command,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Customer> customersWithEmail = await _customerRepository.GetAllAsync(
            1,
            50,
            new CustomerByEmailSpecification(command.Email.EmailAddress),
            cancellationToken
        );
        Customer? customerWithEmail = customersWithEmail?.FirstOrDefault();
        if (customerWithEmail is not null)
        {
            return Result
                .Fail($"A customer with the email '{customerWithEmail.Email.EmailAddress}' already exists.");
        }
        var newCustomer = new Customer(
            command.FullName,
            command.Email,
            command.PhoneNumber
        );
        await _customerRepository.CreateAsync(newCustomer, cancellationToken);
        return Result.Success();
    }
}
