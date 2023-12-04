namespace BookRentalManager.Domain.Specifications;

public sealed class AuthorsBySearchParameterWithBooksSpecification : Specification<Author>
{
    public AuthorsBySearchParameterWithBooksSpecification(string searchParameter, string sortParameters)
    {
        var formattedSearchParameters = searchParameter.Trim().ToLower();
        Where = author => author.FullName.FirstName.Contains(formattedSearchParameters, StringComparison.CurrentCultureIgnoreCase)
            || author.FullName.LastName.Contains(formattedSearchParameters, StringComparison.CurrentCultureIgnoreCase);
        IncludeExpressions.Add(author => author.Books);
        OrderByPropertyName = sortParameters;
    }
}
