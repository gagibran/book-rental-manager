namespace BookRentalManager.Domain.Specifications;

public sealed class AuthorByIdSpecification : Specification<Author>
{
    public AuthorByIdSpecification(Guid id)
    {
        Where = author => author.Id == id;
        IncludeExpressions.Add(author => author.Books);
    }
}
