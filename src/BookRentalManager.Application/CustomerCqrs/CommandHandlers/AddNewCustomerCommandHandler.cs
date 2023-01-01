using BookRentalManager.Domain.Entities;
using BookRentalManager.Domain.Interfaces;

namespace BookRentalManager.Application.CustomerCqrs.Handlers;

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
        var newCustomer = new Customer(
            command.FullName,
            command.Email,
            command.PhoneNumber
        );
        IReadOnlyList<Customer> allCustomers = await _customerRepository.GetAllAsync(cancellationToken);
        Customer? customerWithEmail = allCustomers.FirstOrDefault(customer => customer.Email == newCustomer.Email);
        if (customerWithEmail is not null)
        {
            return Result.Fail($"A customer with the email {customerWithEmail.Email} already exists.");
        }

        await _customerRepository.CreateAsync(newCustomer, cancellationToken);
        return Result.Success();
    }
}
