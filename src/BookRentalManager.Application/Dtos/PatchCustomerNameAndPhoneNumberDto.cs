namespace BookRentalManager.Application.Dtos;

public sealed class PatchCustomerNameAndPhoneNumberDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int AreaCode { get; set; }
    public int PrefixAndLineNumber { get; set; }

    public PatchCustomerNameAndPhoneNumberDto(string firstName, string lastName, int areaCode, int prefixAndLineNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        AreaCode = areaCode;
        PrefixAndLineNumber = prefixAndLineNumber;
    }
}
