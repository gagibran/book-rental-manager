namespace BookRentalManager.Domain.ValueObjects;

public sealed class Isbn : ValueObject
{
    public long IsbnNumber { get; } = default!;

    private Isbn()
    {
    }

    private Isbn(long isbnNumber)
    {
        IsbnNumber = isbnNumber;
    }

    public static Result<Isbn> Create(long isbnNumber)
    {
        if (isbnNumber < 1_000_000_000
            || isbnNumber > 9_999_999_999)
        {
            return Result.Fail<Isbn>("Invalid ISBN format.");
        }
        var isbn = new Isbn(isbnNumber);
        return Result.Success<Isbn>(isbn);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsbnNumber;
    }
}
