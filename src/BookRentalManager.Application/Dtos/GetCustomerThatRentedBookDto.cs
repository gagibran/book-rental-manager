namespace BookRentalManager.Application.Dtos;

public sealed class GetCustomerThatRentedBookDto(Customer customer)
{
    public string FullName { get; } = customer.FullName.ToString();
    public string Email { get; } = customer.Email.EmailAddress;
}