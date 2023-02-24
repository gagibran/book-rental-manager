namespace BookRentalManager.Application.Dtos;

public sealed class CustomerCreatedDto
{
    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public int AreaCode { get; }
    public int PrefixAndLineNumber { get; }
    public string CustomerStatus { get; }
    public int CustomerPoints { get; }

    public CustomerCreatedDto(
        Guid id,
        string firstName,
        string lastName,
        string email,
        int areaCode,
        int prefixAndLineNumber,
        string customerStatus,
        int customerPoints)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        AreaCode = areaCode;
        PrefixAndLineNumber = prefixAndLineNumber;
        CustomerStatus = customerStatus;
        CustomerPoints = customerPoints;
    }
}
