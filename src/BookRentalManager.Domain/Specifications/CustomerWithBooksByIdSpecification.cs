namespace BookRentalManager.Domain.Specifications;

public sealed class CustomerWithBooksByIdSpecification : Specification<Customer>
{
    public CustomerWithBooksByIdSpecification(Guid id)
    {
        Where = customer => customer.Id == id;
        IncludeExpressions.Add(customer => customer.Books);
    }
}
