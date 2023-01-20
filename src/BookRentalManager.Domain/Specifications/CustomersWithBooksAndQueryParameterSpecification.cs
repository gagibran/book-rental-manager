namespace BookRentalManager.Domain.Specifications;

public sealed class CustomersWithBooksAndQueryParameterSpecification : Specification<Customer>
{
    public CustomersWithBooksAndQueryParameterSpecification(string queryParameter)
    {
        var foundCustomerType = Enum.TryParse<CustomerType>(queryParameter, true, out CustomerType customerType)
            ? customerType
            : (CustomerType?)null;
        Where(customer =>
            customer.FullName.CompleteName.ToLower().Contains(queryParameter.ToLower())
            || customer.Email.EmailAddress.ToLower().Contains(queryParameter.ToLower())
            || customer.PhoneNumber.CompletePhoneNumber.ToLower().Contains(queryParameter.ToLower())
            || customer.CustomerStatus.CustomerType == foundCustomerType
        );
        Include(customer => customer.Books);
    }
}
