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
        Result firstNameResult = Result.Success();
        Result lastNameResult = Result.Success();
        if (string.IsNullOrWhiteSpace(firstName))
        {
            firstNameResult = Result.Fail<FullName>("firstName", "First name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            lastNameResult = Result.Fail<FullName>("lastName", "Last name cannot be empty.");
        }
        Result combinedResults = Result.Combine(firstNameResult, lastNameResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<FullName>(combinedResults.ErrorType, combinedResults.ErrorMessage);
        }
        return Result.Success<FullName>(new FullName(firstName.Trim() + " " + lastName.Trim()));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return CompleteName;
    }
}
