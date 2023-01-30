namespace BookRentalManager.Domain.Specifications;

public sealed class BookByIdFromAuthorSpecification : Specification<Book>
{
    public BookByIdFromAuthorSpecification(Guid id)
    {
        Where = book => book.Id == id;
        IncludeExpressions.Add(book => book.Authors);
        IncludeExpressions.Add(book => book.Customer!);
    }
}
