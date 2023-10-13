namespace BookRentalManager.Domain.ValueObjects;

public sealed partial class Email : ValueObject
{
    public string EmailAddress { get; }

    [GeneratedRegex("^(\\w|\\d).*@(\\d|\\w)+(\\d|\\w|-)+\\.\\w{2,3}$")]
    private static partial Regex IsEmailValidRegex();

    private Email()
    {
        EmailAddress = string.Empty;
    }

    private Email(string emailAddress)
    {
        EmailAddress = emailAddress;
    }

    public static Result<Email> Create(string emailAddress)
    {
        bool isEmailValid = IsEmailValidRegex().IsMatch(emailAddress);
        if (!isEmailValid)
        {
            return Result.Fail<Email>("email", "Email address is not in a valid format.");
        }
        return Result.Success(new Email(emailAddress.Trim()));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return EmailAddress;
    }
}
