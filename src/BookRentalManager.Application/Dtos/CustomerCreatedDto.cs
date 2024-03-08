namespace BookRentalManager.Application.Dtos;

public sealed record CustomerCreatedDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    int AreaCode,
    int PrefixAndLineNumber,
    string CustomerStatus,
    int CustomerPoints)
    : IdentifiableDto(Id)
{
    public CustomerCreatedDto(Customer customer) : this(
        customer.Id,
        customer.FullName.FirstName,
        customer.FullName.LastName,
        customer.Email.ToString(),
        customer.PhoneNumber.AreaCode,
        customer.PhoneNumber.PrefixAndLineNumber,
        customer.CustomerStatus.CustomerType.ToString(),
        customer.CustomerPoints)
    {
    }
}
