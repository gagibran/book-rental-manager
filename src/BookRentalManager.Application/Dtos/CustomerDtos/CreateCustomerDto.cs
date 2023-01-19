namespace BookRentalManager.Application.Dtos.CustomerDtos;

public sealed class CreateCustomerDto
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public int AreaCode { get; }
    public int PhoneNumber { get; }

    public CreateCustomerDto(
        string firstName,
        string lastName,
        string email,
        int areaCode,
        int phoneNumber
    )
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        AreaCode = areaCode;
        PhoneNumber = phoneNumber;
    }
}
