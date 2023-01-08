namespace BookRentalManager.Domain.Specifications;

public sealed class CustomerWithQueryParameterSpecification : Specification<Customer>
{
    private readonly string _queryParameter;

    public CustomerWithQueryParameterSpecification(string queryParameter)
    {
        _queryParameter = queryParameter;
    }

    public override Expression<Func<Customer, bool>> ToExpression()
    {
        var foundCustomerType = Enum.TryParse<CustomerType>(_queryParameter, true, out CustomerType customerType)
            ? customerType
            : (CustomerType?)null;
        return customer =>
            customer.FullName.CompleteName.ToLower().Contains(_queryParameter.ToLower())
            || customer.Email.EmailAddress.ToLower().Contains(_queryParameter.ToLower())
            || customer.PhoneNumber.CompletePhoneNumber.ToLower().Contains(_queryParameter.ToLower())
            || customer.CustomerStatus.CustomerType == foundCustomerType;
    }
}
