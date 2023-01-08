namespace BookRentalManager.Application.Dtos;

public sealed class GetCustomerDto
{
    public Guid Id { get; }
    public string FullName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public IReadOnlyList<GetCustomerBooksDto> Books { get; }
    public string CustomerStatus { get; }
    public int CustomerPoints { get; }

    public GetCustomerDto(
        Guid id,
        FullName fullName,
        Email email,
        PhoneNumber phoneNumber,
        IReadOnlyList<GetCustomerBooksDto> books,
        CustomerStatus customerStatus,
        int customerPoints
    )
    {
        Id = id;
        FullName = fullName.CompleteName;
        Email = email.EmailAddress;
        PhoneNumber = phoneNumber.CompletePhoneNumber;
        Books = books;
        CustomerStatus = customerStatus.CustomerType.ToString();
        CustomerPoints = customerPoints;
    }
}