namespace BookRentalManager.Domain.Specifications;

public sealed class CustomerByIdWithBooksSpecification : Specification<Customer>
{
    public CustomerByIdWithBooksSpecification(Guid id)
    {
        Where = customer => customer.Id == id;
        IncludeExpressions.Add(customer => customer.Books);
    }
}
