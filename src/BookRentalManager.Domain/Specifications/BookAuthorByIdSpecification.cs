namespace BookRentalManager.Domain.Specifications;

public sealed class BookAuthorByIdSpecification : Specification<BookAuthor>
{
    public BookAuthorByIdSpecification(Guid id)
    {
        Where = bookAuthor => bookAuthor.Id == id;
        IncludeExpressions.Add(bookAuthor => bookAuthor.Books);
    }
}
