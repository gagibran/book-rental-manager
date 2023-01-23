namespace BookRentalManager.Application.Mappers;

internal sealed class GetRentedByDtoMapper : IMapper<Customer?, GetRentedByDto>
{
    public GetRentedByDto Map(Customer? customer)
    {
        if (customer is null)
        {
            return new GetRentedByDto();
        }
        return new GetRentedByDto(customer.FullName, customer.Email);
    }
}