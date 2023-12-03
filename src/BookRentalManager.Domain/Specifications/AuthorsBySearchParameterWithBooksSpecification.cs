namespace BookRentalManager.Domain.Specifications;

public sealed class AuthorsBySearchParameterWithBooksSpecification : Specification<Author>
{
    public AuthorsBySearchParameterWithBooksSpecification(string searchParameter, string sortParameters)
    {
        var formattedSearchParameters = searchParameter.Trim().ToLower();
        Where = author => author.FullName.FirstName.ToLower().Contains(formattedSearchParameters)
            || author.FullName.LastName.ToLower().Contains(formattedSearchParameters);
        IncludeExpressions.Add(author => author.Books);
        OrderByPropertyName = sortParameters;
    }
}
