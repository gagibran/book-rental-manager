namespace BookRentalManager.Application.Dtos;

public sealed class GetCustomerDto
{
    public Guid Id { get; }
    public FullName FullName { get; }
    public Email Email { get; }
    public PhoneNumber PhoneNumber { get; }
    public IReadOnlyList<GetCustomerBooksDto> Books { get; }
    public CustomerStatus CustomerStatus { get; }
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
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        Books = books;
        CustomerStatus = customerStatus;
        CustomerPoints = customerPoints;
    }
}
