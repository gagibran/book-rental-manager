namespace BookRentalManager.Application.Dtos;

public sealed class GetCustomerDto : SingleResourceBaseDto
{
    public string FullName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public IReadOnlyList<GetBookRentedByCustomerDto> Books { get; }
    public string CustomerStatus { get; }
    public int CustomerPoints { get; }

    public GetCustomerDto(
        Guid id,
        FullName fullName,
        Email email,
        PhoneNumber phoneNumber,
        IReadOnlyList<GetBookRentedByCustomerDto> books,
        CustomerStatus customerStatus,
        int customerPoints) : base(id)
    {
        FullName = fullName.ToString();
        Email = email.EmailAddress;
        PhoneNumber = phoneNumber.GetCompletePhoneNumber();
        Books = books;
        CustomerStatus = customerStatus.CustomerType.ToString();
        CustomerPoints = customerPoints;
    }
}
