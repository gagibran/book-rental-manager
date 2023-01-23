namespace BookRentalManager.Domain.Specifications;

public sealed class CustomerByIdSpecification : Specification<Customer>
{
    public CustomerByIdSpecification(Guid id)
    {
        Where = customer => customer.Id == id;
        IncludeExpressions.Add(customer => customer.Books);
    }
}
