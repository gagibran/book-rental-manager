namespace BookRentalManager.Application.Dtos;

public sealed class GetCustomerDto(
    Guid id,
    FullName fullName,
    Email email,
    PhoneNumber phoneNumber,
    IReadOnlyList<GetBookRentedByCustomerDto> books,
    CustomerStatus customerStatus,
    int customerPoints) : IdentifiableDto(id)
{
    public string FullName { get; } = fullName.ToString();
    public string Email { get; } = email.EmailAddress;
    public string PhoneNumber { get; } = phoneNumber.GetCompletePhoneNumber();
    public IReadOnlyList<GetBookRentedByCustomerDto> Books { get; } = books;
    public string CustomerStatus { get; } = customerStatus.CustomerType.ToString();
    public int CustomerPoints { get; } = customerPoints;
}
