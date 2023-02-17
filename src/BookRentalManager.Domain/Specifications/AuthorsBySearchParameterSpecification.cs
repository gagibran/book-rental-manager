namespace BookRentalManager.Domain.Specifications;

public sealed class AuthorsBySearchParameterSpecification : Specification<Author>
{
    public AuthorsBySearchParameterSpecification(string searchParameter, string sortParameter)
    {
        Where = author => author.FullName.CompleteName.ToLower().Contains(searchParameter.Trim().ToLower());
        IncludeExpressions.Add(author => author.Books);
        OrderByPropertyName = sortParameter;
    }
}
