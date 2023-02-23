namespace BookRentalManager.Domain.Specifications;

public sealed class CustomersBySearchParameterWithBooksSpecification : Specification<Customer>
{
    public CustomersBySearchParameterWithBooksSpecification(string searchParameter, string sortParameters)
    {
        var formattedSearchParameter = searchParameter.ToLower().Trim();
        var foundCustomerType = (CustomerType?)null;
        if (Enum.TryParse<CustomerType>(formattedSearchParameter, true, out CustomerType customerType))
        {
            foundCustomerType = customerType;
        }
        Where = customer =>
            customer.FullName.CompleteName.ToLower().Contains(formattedSearchParameter)
            || customer.Email.EmailAddress.ToLower().Contains(formattedSearchParameter)
            || customer.PhoneNumber.CompletePhoneNumber.ToLower().Contains(formattedSearchParameter)
            || customer.CustomerStatus.CustomerType == foundCustomerType;
        IncludeExpressions.Add(customer => customer.Books);
        OrderByPropertyName = sortParameters;
    }
}
