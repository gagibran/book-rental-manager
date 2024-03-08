namespace BookRentalManager.Application.Customers.Commands;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    PhoneNumberDto PhoneNumber)
    : IRequest<CustomerCreatedDto>;
