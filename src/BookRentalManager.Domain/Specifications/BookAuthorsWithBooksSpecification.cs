namespace BookRentalManager.Domain.Specifications;

public sealed class BookAuthorsWithBooksSpecification : Specification<BookAuthor>
{
    public BookAuthorsWithBooksSpecification()
    {
        Include(bookAuthor => bookAuthor.Books);
    }
}
