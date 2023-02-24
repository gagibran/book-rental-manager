namespace BookRentalManager.Domain.Specifications;

public sealed class CustomersBySearchParameterWithBooksSpecification : Specification<Customer>
{
    public CustomersBySearchParameterWithBooksSpecification(string searchParameter, string sortParameters)
    {
        var formattedSearchParameter = searchParameter.ToLower().Trim();
        Where = customer =>
            customer.FullName.FirstName.ToLower().Contains(formattedSearchParameter)
            || customer.FullName.LastName.ToLower().Contains(formattedSearchParameter)
            || customer.Email.EmailAddress.ToLower().Contains(formattedSearchParameter);
        IncludeExpressions.Add(customer => customer.Books);
        OrderByPropertyName = sortParameters;
    }
}
