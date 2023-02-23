namespace BookRentalManager.Domain.Specifications;

public sealed class AuthorByIdWithBooksSpecification : Specification<Author>
{
    public AuthorByIdWithBooksSpecification(Guid id)
    {
        Where = author => author.Id == id;
        IncludeExpressions.Add(author => author.Books);
    }
}
