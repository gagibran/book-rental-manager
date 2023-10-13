namespace BookRentalManager.Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private const int MinAreaCode = 200;
    private const int MaxAreaCode = 999;
    private const int MinPrefixAndLineNumber = 2_000_000;
    private const int MaxPrefixAndLineNumber = 9_999_999;

    public int AreaCode { get; }
    public int PrefixAndLineNumber { get; }

    private PhoneNumber()
    {
    }

    private PhoneNumber(int areaCode, int prefixAndLineNumber)
    {
        AreaCode = areaCode;
        PrefixAndLineNumber = prefixAndLineNumber;
    }

    public static Result<PhoneNumber> Create(int areaCode, int prefixAndLineNumber)
    {
        Result finalResult = Result.Success();
        if (areaCode < MinAreaCode || areaCode > MaxAreaCode)
        {
            finalResult = Result.Fail<PhoneNumber>("invalidAreaCode", "Invalid area code.");
        }
        if (prefixAndLineNumber < MinPrefixAndLineNumber || prefixAndLineNumber > MaxPrefixAndLineNumber)
        {
            finalResult = Result.Combine(finalResult, Result.Fail<PhoneNumber>("invalidPhoneNumber", "Invalid phone number."));
        }
        if (!finalResult.IsSuccess)
        {
            return Result.Fail<PhoneNumber>(finalResult.ErrorType, finalResult.ErrorMessage);
        }
        return Result.Success(new PhoneNumber(areaCode, prefixAndLineNumber));
    }

    public string GetCompletePhoneNumber()
    {
        return $"+1{AreaCode}{PrefixAndLineNumber}";
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return AreaCode;
        yield return PrefixAndLineNumber;
    }
}
