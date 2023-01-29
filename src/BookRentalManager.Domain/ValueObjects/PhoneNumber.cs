namespace BookRentalManager.Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private const int MinAreaCode = 200;
    private const int MaxAreaCode = 999;
    private const int MinPrefixAndLineNumber = 2_000_000;
    private const int MaxPrefixAndLineNumber = 9_999_999;

    public string CompletePhoneNumber { get; }

    private PhoneNumber()
    {
        CompletePhoneNumber = string.Empty;
    }

    private PhoneNumber(int areaCode, int prefixAndLineNumber)
    {
        CompletePhoneNumber = $"+1{areaCode}{prefixAndLineNumber}";
    }

    public static Result<PhoneNumber> Create(int areaCode, int prefixAndLineNumber)
    {
        if (areaCode < MinAreaCode || areaCode > MaxAreaCode)
        {
            return Result.Fail<PhoneNumber>(nameof(Create), "Invalid area code.");
        }
        if (prefixAndLineNumber < MinPrefixAndLineNumber || prefixAndLineNumber > MaxPrefixAndLineNumber)
        {
            return Result.Fail<PhoneNumber>(nameof(Create), "Invalid phone number.");
        }
        return Result.Success<PhoneNumber>(new PhoneNumber(areaCode, prefixAndLineNumber));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return CompletePhoneNumber;
    }
}
