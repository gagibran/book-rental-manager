namespace BookRentalManager.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string EmailAddress { get; }

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
        bool isEmailValid = Regex.IsMatch(emailAddress, @"^(\w|\d).*@(\d|\w)+(\d|\w|-)+\.\w{2,3}$");
        if (!isEmailValid)
        {
            return Result.Fail<Email>("Email address is not in a valid format.");
        }
        return Result.Success<Email>(new Email(emailAddress.Trim()));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return EmailAddress;
    }
}
