namespace BookRentalManager.Domain.Specifications;

public sealed class BooksByIdsSpecification : Specification<Book>
{
    public BooksByIdsSpecification(IEnumerable<Guid> ids)
    {
        Where = book => ids.Contains(book.Id);
    }
}
