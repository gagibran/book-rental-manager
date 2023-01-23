namespace BookRentalManager.Domain.Specifications;

public sealed class CustomerByEmailSpecification : Specification<Customer>
{

    public CustomerByEmailSpecification(string email)
    {
        Where = customer => customer.Email.EmailAddress == email;
        IncludeExpressions.Add(customer => customer.Books);
    }
}
