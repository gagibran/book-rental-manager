namespace BookRentalManager.Domain.Specifications;

public sealed class BooksByIsbnsSpecification : Specification<Book>
{
    public BooksByIsbnsSpecification(List<string> isbns)
    {
        Where = book => isbns.Contains(book.Isbn.IsbnValue);
    }
}
