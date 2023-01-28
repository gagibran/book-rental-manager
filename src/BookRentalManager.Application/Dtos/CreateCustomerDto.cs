namespace BookRentalManager.Application.Dtos;

public sealed class CreateCustomerDto
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public PhoneNumberDto PhoneNumber { get; }

    public CreateCustomerDto(
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
