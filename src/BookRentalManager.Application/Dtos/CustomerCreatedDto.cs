namespace BookRentalManager.Application.Dtos;

public sealed class CustomerCreatedDto(
    Guid id,
    string firstName,
    string lastName,
    string email,
    int areaCode,
    int prefixAndLineNumber,
    string customerStatus,
    int customerPoints)
    : IdentifiableDto(id)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string Email { get; } = email;
    public int AreaCode { get; } = areaCode;
    public int PrefixAndLineNumber { get; } = prefixAndLineNumber;
    public string CustomerStatus { get; } = customerStatus;
    public int CustomerPoints { get; } = customerPoints;
}
