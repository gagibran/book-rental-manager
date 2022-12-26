namespace SimpleBookManagement.Domain.ValueObjects;

public sealed class Isbn : ValueObject
{
    public long IsbnNumber { get; }

    private Isbn(long isbnNumber)
    {
        IsbnNumber = isbnNumber;
    }

    public static Result<Isbn> Create(long isbnNumber)
    {
        if (isbnNumber < 1_000_000_000
            || isbnNumber > 9_999_999_999)
        {
            return Result<Isbn>.Fail("Invalid ISBN format.");
        }
        var isbn = new Isbn(isbnNumber);
        return Result<Isbn>.Success(isbn);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsbnNumber;
    }
}
