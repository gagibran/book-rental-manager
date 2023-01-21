namespace BookRentalManager.Domain.Specifications;

public sealed class CustomersWithBooksSpecification : Specification<Customer>
{
    public CustomersWithBooksSpecification()
    {
        IncludeExpressions.Add(customer => customer.Books);
    }
}
