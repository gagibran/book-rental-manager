namespace BookRentalManager.Domain.Specifications;

public sealed class BookInBookAuthorBooksByIsbnSpecification : Specification<Book>
{
    public BookInBookAuthorBooksByIsbnSpecification(IReadOnlyList<Book> books, string isbn)
    {
        Where = book => books.Contains(book) && book.Isbn.IsbnValue.Equals(isbn);
    }
}
