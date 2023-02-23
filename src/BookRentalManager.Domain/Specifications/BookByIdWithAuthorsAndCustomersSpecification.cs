namespace BookRentalManager.Domain.Specifications;

public sealed class BookByIdWithAuthorsAndCustomersSpecification : Specification<Book>
{
    public BookByIdWithAuthorsAndCustomersSpecification(Guid id)
    {
        Where = book => book.Id == id;
        IncludeExpressions.Add(book => book.Authors);
        IncludeExpressions.Add(book => book.Customer!);
    }
}
