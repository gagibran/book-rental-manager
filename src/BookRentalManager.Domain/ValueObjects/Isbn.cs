namespace BookRentalManager.Domain.ValueObjects;

public sealed partial class Isbn : ValueObject
{
    public string IsbnValue { get; }

    [GeneratedRegex("\\s+|-+")]
    private static partial Regex FormattedIsbn();

    private Isbn()
    {
        IsbnValue = string.Empty;
    }

    private Isbn(string isbnValue)
    {
        IsbnValue = isbnValue;
    }

    public static Result<Isbn> Create(string isbnValue)
    {
        string formattedIsbn = FormattedIsbn().Replace(isbnValue, "").ToUpper();
        if (formattedIsbn.Length != 10 && formattedIsbn.Length != 13)
        {
            return Result.Fail<Isbn>("isbnFormat", "Invalid ISBN format.");
        }
        return Result.Success(new Isbn(isbnValue.Trim()));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsbnValue;
    }

    public override string ToString()
    {
        return IsbnValue;
    }
}
