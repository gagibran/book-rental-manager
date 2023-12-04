namespace BookRentalManager.Application.Dtos;

public sealed class PatchCustomerNameAndPhoneNumberDto(string firstName, string lastName, int areaCode, int prefixAndLineNumber)
{
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public int AreaCode { get; set; } = areaCode;
    public int PrefixAndLineNumber { get; set; } = prefixAndLineNumber;
}
