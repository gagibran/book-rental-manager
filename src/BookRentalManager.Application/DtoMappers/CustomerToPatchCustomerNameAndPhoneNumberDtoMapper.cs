namespace BookRentalManager.Application.DtoMappers;

internal sealed class CustomerToPatchCustomerNameAndPhoneNumberDtoMapper : IMapper<Customer, PatchCustomerNameAndPhoneNumberDto>
{
    public PatchCustomerNameAndPhoneNumberDto Map(Customer customer)
    {
        return new PatchCustomerNameAndPhoneNumberDto(
            customer.FullName.FirstName,
            customer.FullName.LastName,
            customer.PhoneNumber.AreaCode,
            customer.PhoneNumber.PrefixAndLineNumber);
    }
}
