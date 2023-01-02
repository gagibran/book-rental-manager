using BookRentalManager.Domain.Entities;
using BookRentalManager.Domain.Interfaces;
using BookRentalManager.Domain.Specifications;

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
        Customer? customerWithEmail = await _customerRepository.GetBySpecificationAsync(
            new CustomerByEmailSpecification(command.Email),
            cancellationToken
        );
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
