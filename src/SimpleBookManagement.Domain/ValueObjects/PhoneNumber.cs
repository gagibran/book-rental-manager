namespace SimpleBookManagement.Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private const int MinAreaCode = 200;
    private const int MaxAreaCode = 999;
    private const int MinActualPhoneNumber = 2_000_000;
    private const int MaxActualPhoneNumber = 9_999_999;

    public string CompletePhoneNumber { get; } = default!;

    public PhoneNumber()
    {
    }

    private PhoneNumber(int areaCode, int actualPhoneNumber)
    {
        CompletePhoneNumber = $"+1{areaCode}{actualPhoneNumber}";
    }

    public static Result<PhoneNumber> Create(int areaCode, int actualPhoneNumber)
    {
        if (areaCode < MinAreaCode || areaCode > MaxAreaCode)
        {
            return Result.Fail<PhoneNumber>("Invalid area code.");
        }
        if (actualPhoneNumber < MinActualPhoneNumber
            || actualPhoneNumber > MaxActualPhoneNumber)
        {
            return Result.Fail<PhoneNumber>("Invalid phone number.");
        }
        return Result.Success<PhoneNumber>(
            new PhoneNumber(areaCode, actualPhoneNumber)
        );
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return CompletePhoneNumber;
    }
}
