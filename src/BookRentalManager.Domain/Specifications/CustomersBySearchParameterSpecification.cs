namespace BookRentalManager.Domain.Specifications;

public sealed class CustomersBySearchParameterSpecification : Specification<Customer>
{
    public CustomersBySearchParameterSpecification(string searchParameter)
    {
        var formattedSearchParameter = searchParameter.ToLower().Trim();
        var foundCustomerType = Enum.TryParse<CustomerType>(formattedSearchParameter, true, out CustomerType customerType)
            ? customerType
            : (CustomerType?)null;
        Where = customer =>
            customer.FullName.CompleteName.ToLower().Contains(formattedSearchParameter)
            || customer.Email.EmailAddress.ToLower().Contains(formattedSearchParameter)
            || customer.PhoneNumber.CompletePhoneNumber.ToLower().Contains(formattedSearchParameter)
            || customer.CustomerStatus.CustomerType == foundCustomerType;
        IncludeExpressions.Add(customer => customer.Books);
    }
}
