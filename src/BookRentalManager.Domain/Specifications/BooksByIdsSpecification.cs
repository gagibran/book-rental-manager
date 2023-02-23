namespace BookRentalManager.Domain.Specifications;

public sealed class BooksByIdsSpecification : Specification<Book>
{
    public BooksByIdsSpecification(IReadOnlyList<Guid> ids)
    {
        Where = book => ids.Contains(book.Id);
    }
}
