namespace BookRentalManager.Application.Dtos;

public sealed class PhoneNumberDto(int areaCode, int prefixAndLineNumber)
{
    public int AreaCode { get; } = areaCode;
    public int PrefixAndLineNumber { get; } = prefixAndLineNumber;
}
