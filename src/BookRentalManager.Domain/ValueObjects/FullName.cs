namespace BookRentalManager.Domain.ValueObjects;

public sealed class FullName : ValueObject
{
    public string CompleteName { get; }

    private FullName()
    {
        CompleteName = string.Empty;
    }

    private FullName(string completeName)
    {
        CompleteName = completeName;
    }

    public static Result<FullName> Create(string firstName, string lastName)
    {
        Result finalResult = Result.Success();
        if (string.IsNullOrWhiteSpace(firstName))
        {
            finalResult = Result.Fail<FullName>("firstName", "First name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            finalResult = Result.Combine(finalResult, Result.Fail<FullName>("lastName", "Last name cannot be empty."));
        }
        if (!finalResult.IsSuccess)
        {
            return Result.Fail<FullName>(finalResult.ErrorType, finalResult.ErrorMessage);
        }
        return Result.Success<FullName>(new FullName(firstName.Trim() + " " + lastName.Trim()));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return CompleteName;
    }
}
