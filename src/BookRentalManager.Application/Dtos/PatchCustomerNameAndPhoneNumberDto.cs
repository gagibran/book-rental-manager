namespace BookRentalManager.Application.Dtos;

public sealed record PatchCustomerNameAndPhoneNumberDto(string FirstName, string LastName, int AreaCode, int PrefixAndLineNumber)
{
    public PatchCustomerNameAndPhoneNumberDto(Customer customer) : this(
        customer.FullName.FirstName,
        customer.FullName.LastName,
        customer.PhoneNumber.AreaCode,
        customer.PhoneNumber.PrefixAndLineNumber)
    {
    }
}
