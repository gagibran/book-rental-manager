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
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Fail<FullName>(nameof(Create), "First name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Fail<FullName>(nameof(Create), "Last name cannot be empty.");
        }
        return Result.Success<FullName>(new FullName(firstName.Trim() + " " + lastName.Trim()));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return CompleteName;
    }
}
