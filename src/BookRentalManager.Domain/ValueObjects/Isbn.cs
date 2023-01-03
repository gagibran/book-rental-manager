using System.Text.RegularExpressions;

namespace BookRentalManager.Domain.ValueObjects;

public sealed class Isbn : ValueObject
{
    public string IsbnValue { get; } = default!;

    private Isbn()
    {
    }

    private Isbn(string isbnValue)
    {
        IsbnValue = isbnValue;
    }

    public static Result<Isbn> Create(string isbnValue)
    {
        string formattedIsbn = Regex.Replace(isbnValue, @"\s+|-+", "").ToUpper();
        if (formattedIsbn.Length != 10 && formattedIsbn.Length != 13)
        {
            return Result.Fail<Isbn>("Invalid ISBN format.");
        }
        var isbn = new Isbn(formattedIsbn);
        return Result.Success<Isbn>(isbn);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsbnValue;
    }
}
