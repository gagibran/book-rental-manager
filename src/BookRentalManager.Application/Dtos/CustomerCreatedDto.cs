namespace BookRentalManager.Application.Dtos;

public sealed class CustomerCreatedDto
{
    public Guid Id { get; }
    public string FullName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public string CustomerStatus { get; }
    public int CustomerPoints { get; }

    public CustomerCreatedDto(
        Guid id,
        string fullName,
        string email,
        string phoneNumber,
        string customerStatus,
        int customerPoints)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        CustomerStatus = customerStatus;
        CustomerPoints = customerPoints;
    }
}
