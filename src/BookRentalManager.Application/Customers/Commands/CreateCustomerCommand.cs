namespace BookRentalManager.Application.Customers.Commands;

public sealed class CreateCustomerCommand(
    string firstName,
    string lastName,
    string email,
    PhoneNumberDto phoneNumber)
    : IRequest<CustomerCreatedDto>
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string Email { get; } = email;
    public PhoneNumberDto PhoneNumber { get; } = phoneNumber;
}
