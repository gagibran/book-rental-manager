namespace BookRentalManager.Domain.ValueObjects;

public sealed class BookTitle : ValueObject
{
    public string Title { get; set; }

    private BookTitle()
    {
        Title = default!;
    }

    private BookTitle(string title)
    {
        Title = title;
    }

    public static Result<BookTitle> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Fail<BookTitle>("bookTitle", "The title can't be empty.");
        }
        return Result.Success(new BookTitle(title.Trim()));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Title;
    }
}
