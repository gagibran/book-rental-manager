namespace BookRentalManager.Application.Dtos;

public sealed class GetCustomerThatRentedBookDto
{
    public string FullName { get; }
    public string Email { get; }

    public GetCustomerThatRentedBookDto(Customer customer)
    {
        FullName = customer.FullName.ToString();
        Email = customer.Email.EmailAddress;
    }
}