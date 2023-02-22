namespace BookRentalManager.Domain.Specifications;

public sealed class BookByIsbnSpecification : Specification<Book>
{
    public BookByIsbnSpecification(string isbn)
    {
        Where = book => book.Isbn.IsbnValue.Equals(isbn);
    }
}
