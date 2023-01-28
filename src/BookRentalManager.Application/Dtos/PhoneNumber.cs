namespace BookRentalManager.Application.Dtos;

public sealed class PhoneNumberDto
{
    public int AreaCode { get; }
    public int PrefixAndLineNumber { get; }

    public PhoneNumberDto(int areaCode, int prefixAndLineNumber)
    {
        AreaCode = areaCode;
        PrefixAndLineNumber = prefixAndLineNumber;
    }
}
