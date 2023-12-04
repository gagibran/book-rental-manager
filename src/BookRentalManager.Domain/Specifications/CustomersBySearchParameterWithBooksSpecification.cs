namespace BookRentalManager.Domain.Specifications;

public sealed class CustomersBySearchParameterWithBooksSpecification : Specification<Customer>
{
    public CustomersBySearchParameterWithBooksSpecification(string searchParameter, string sortParameters)
    {
        var formattedSearchParameter = searchParameter.ToLower().Trim();
        Where = customer =>
            customer.FullName.FirstName.Contains(formattedSearchParameter, StringComparison.CurrentCultureIgnoreCase)
            || customer.FullName.LastName.Contains(formattedSearchParameter, StringComparison.CurrentCultureIgnoreCase)
            || customer.Email.EmailAddress.Contains(formattedSearchParameter, StringComparison.CurrentCultureIgnoreCase);
        IncludeExpressions.Add(customer => customer.Books);
        OrderByPropertyName = sortParameters;
    }
}
