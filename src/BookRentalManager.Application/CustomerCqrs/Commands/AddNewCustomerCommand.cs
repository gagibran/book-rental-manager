namespace BookRentalManager.Application.CustomerCqrs.Commands;

public sealed class AddNewCustomerCommand : ICommand
{
    public FullName FullName { get; }
    public Email Email { get; }
    public PhoneNumber PhoneNumber { get; }

    public AddNewCustomerCommand(
        FullName fullName,
        Email email,
        PhoneNumber phoneNumber
    )
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}
