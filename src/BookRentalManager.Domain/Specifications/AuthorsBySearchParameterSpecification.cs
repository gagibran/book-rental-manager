namespace BookRentalManager.Domain.Specifications;

public sealed class AuthorsBySearchParameterSpecification : Specification<Author>
{
    public AuthorsBySearchParameterSpecification(string searchParameter, string sortParameters)
    {
        Where = author => author.FullName.CompleteName.ToLower().Contains(searchParameter.Trim().ToLower());
        IncludeExpressions.Add(author => author.Books);
        OrderByPropertyName = sortParameters;
    }
}
