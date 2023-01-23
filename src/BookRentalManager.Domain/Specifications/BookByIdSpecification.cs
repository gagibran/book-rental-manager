namespace BookRentalManager.Domain.Specifications;

public sealed class BookByIdSpecification : Specification<Book>
{
    public BookByIdSpecification(Guid id)
    {
        Where = book => book.Id == id;
        IncludeExpressions.Add(book => book.BookAuthors);
        IncludeExpressions.Add(book => book.Customer!);
    }
}
