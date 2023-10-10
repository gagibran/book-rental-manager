namespace BookRentalManager.Application.DtoMappers;

internal sealed class CustomerToGetCustomerThatRentedBookDtoMapper : IMapper<Customer?, GetCustomerThatRentedBookDto>
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
