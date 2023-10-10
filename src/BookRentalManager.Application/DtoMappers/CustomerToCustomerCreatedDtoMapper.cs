namespace BookRentalManager.Application.DtoMappers;

internal sealed class CustomerToCustomerCreatedDtoMapper : IMapper<Customer, CustomerCreatedDto>
{
    public CustomerCreatedDto Map(Customer customer)
    {
        return new CustomerCreatedDto(
            customer.Id,
            customer.FullName.FirstName,
            customer.FullName.LastName,
            customer.Email.EmailAddress,
            customer.PhoneNumber.AreaCode,
            customer.PhoneNumber.PrefixAndLineNumber,
            customer.CustomerStatus.CustomerType.ToString(),
            customer.CustomerPoints);
    }
}
