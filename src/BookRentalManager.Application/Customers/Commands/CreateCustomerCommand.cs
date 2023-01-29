namespace BookRentalManager.Application.Customers.Commands;

public sealed class CreateCustomerCommand : ICommand<CustomerCreatedDto>
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public int AreaCode { get; }
    public int PrefixAndLineNumber { get; }

    public CreateCustomerCommand(
        string firstName,
        string lastName,
        string email,
        int areaCode,
        int prefixAndLineNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        AreaCode = areaCode;
        PrefixAndLineNumber = prefixAndLineNumber;
    }
}
