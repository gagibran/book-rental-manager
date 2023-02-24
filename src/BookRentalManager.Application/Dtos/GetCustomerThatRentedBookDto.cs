namespace BookRentalManager.Application.Dtos;

public sealed class GetCustomerThatRentedBookDto
{
    public string? FullName { get; }
    public string? Email { get; }

    public GetCustomerThatRentedBookDto()
    {
    }

    public GetCustomerThatRentedBookDto(FullName fullName, Email email)
    {
        FullName = fullName.GetFullName();
        Email = email.EmailAddress;
    }
}