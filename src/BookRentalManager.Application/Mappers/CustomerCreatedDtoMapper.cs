namespace BookRentalManager.Application.Mappers;

internal sealed class CustomerCreatedDtoMapper : IMapper<Customer, CustomerCreatedDto>
{
    public CustomerCreatedDto Map(Customer customer)
    {
        return new CustomerCreatedDto(
            customer.Id,
            customer.FullName.CompleteName,
            customer.Email.EmailAddress,
            customer.PhoneNumber.CompletePhoneNumber,
            customer.CustomerStatus.CustomerType.ToString(),
            customer.CustomerPoints);
    }
}
