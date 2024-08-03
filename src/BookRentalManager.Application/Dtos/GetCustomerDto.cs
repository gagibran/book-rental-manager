namespace BookRentalManager.Application.Dtos;

[method: JsonConstructor]
public sealed record GetCustomerDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    IReadOnlyList<GetBookRentedByCustomerDto> Books,
    string CustomerStatus,
    int CustomerPoints)
    : IdentifiableDto(Id)
{
    public GetCustomerDto(Customer customer) : this(
        customer.Id,
        customer.FullName.ToString(),
        customer.Email.ToString(),
        customer.PhoneNumber.ToString(),
        customer
            .Books
            .Select(book => new GetBookRentedByCustomerDto(book))
            .ToList()
            .AsReadOnly(),
        customer.CustomerStatus.ToString(),
        customer.CustomerPoints)
    {
    }
}
