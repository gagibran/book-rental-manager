namespace BookRentalManager.Domain.Specifications;

public sealed class BookInBookAuthorBooksByTitleSpecification : Specification<Book>
{
    public BookInBookAuthorBooksByTitleSpecification(IReadOnlyList<Book> books, string bookTitle)
    {
        Where = book => books.Contains(book) && book.BookTitle == bookTitle;
    }
}
