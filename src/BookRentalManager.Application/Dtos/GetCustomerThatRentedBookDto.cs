namespace BookRentalManager.Application.Dtos;

[method: JsonConstructor]
public sealed record GetCustomerThatRentedBookDto(string FullName, string Email);
