namespace BookRentalManager.Domain.Specifications;

public sealed class CustomersWithBooksSpecification : Specification<Customer>
{
    public CustomersWithBooksSpecification()
    {
        Include(customer => customer.Books);
    }
}
