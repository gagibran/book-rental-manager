namespace SimpleBookManagement.Domain.ValueObjects;

public sealed class FullName : ValueObject
{
    public string CompleteName { get; }

    private FullName(string completeName)
    {
        CompleteName = completeName;
    }

    public static Result<FullName> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<FullName>.Fail("First name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<FullName>.Fail("Last name cannot be empty.");
        }
        return Result<FullName>.Success(new FullName(firstName + "" + lastName));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return CompleteName;
    }
}
