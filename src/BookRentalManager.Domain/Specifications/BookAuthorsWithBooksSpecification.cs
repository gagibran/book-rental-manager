namespace BookRentalManager.Domain.Specifications;

public sealed class BookAuthorsWithBooksSpecification : Specification<BookAuthor>
{
    public BookAuthorsWithBooksSpecification()
    {
        IncludeExpressions.Add(bookAuthor => bookAuthor.Books);
    }
}
