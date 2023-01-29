namespace BookRentalManager.Application.Customers.Commands;

public sealed class CreateCustomerCommand : ICommand<CustomerCreatedDto>
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public PhoneNumberDto PhoneNumber { get; }

    public CreateCustomerCommand(
        string firstName,
        string lastName,
        string email,
        PhoneNumberDto phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}
