namespace BookRentalManager.Application.Dtos;

public sealed class PatchCustomerNameAndPhoneNumberDto(string firstName, string lastName, int areaCode, int prefixAndLineNumber)
{
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public int AreaCode { get; set; } = areaCode;
    public int PrefixAndLineNumber { get; set; } = prefixAndLineNumber;

    public PatchCustomerNameAndPhoneNumberDto(Customer customer) : this(
        customer.FullName.FirstName,
        customer.FullName.LastName,
        customer.PhoneNumber.AreaCode,
        customer.PhoneNumber.PrefixAndLineNumber)
    {
    }
}
