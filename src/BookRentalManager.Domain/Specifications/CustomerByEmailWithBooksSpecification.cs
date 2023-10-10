namespace BookRentalManager.Domain.Specifications;

public sealed class CustomerByEmailWithBooksSpecification : Specification<Customer>
{

    public CustomerByEmailWithBooksSpecification(string email)
    {
        Where = customer => customer.Email.EmailAddress == email;
        IncludeExpressions.Add(customer => customer.Books);
    }
}
