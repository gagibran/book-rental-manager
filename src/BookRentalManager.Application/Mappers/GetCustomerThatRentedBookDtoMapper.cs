namespace BookRentalManager.Application.Mappers;

internal sealed class GetCustomerThatRentedBookDtoMapper : IMapper<Customer?, GetCustomerThatRentedBookDto>
{
    public GetCustomerThatRentedBookDto Map(Customer? customer)
    {
        if (customer is null)
        {
            return new GetCustomerThatRentedBookDto();
        }
        return new GetCustomerThatRentedBookDto(customer.FullName, customer.Email);
    }
}
