namespace BookRentalManager.Domain.Specifications;

public sealed class CustomersWithBooksAndSearchParamSpecification : Specification<Customer>
{
    public CustomersWithBooksAndSearchParamSpecification(string searchParameter)
    {
        var foundCustomerType = Enum.TryParse<CustomerType>(searchParameter, true, out CustomerType customerType)
            ? customerType
            : (CustomerType?)null;
        Where = customer =>
            customer.FullName.CompleteName.ToLower().Contains(searchParameter.ToLower())
            || customer.Email.EmailAddress.ToLower().Contains(searchParameter.ToLower())
            || customer.PhoneNumber.CompletePhoneNumber.ToLower().Contains(searchParameter.ToLower())
            || customer.CustomerStatus.CustomerType == foundCustomerType;
        IncludeExpressions.Add(customer => customer.Books);
    }
}
