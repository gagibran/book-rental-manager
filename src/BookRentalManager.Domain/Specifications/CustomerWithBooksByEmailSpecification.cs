namespace BookRentalManager.Domain.Specifications;

public sealed class CustomerWithBooksByEmailSpecification : Specification<Customer>
{

    public CustomerWithBooksByEmailSpecification(string email)
    {
        Where(customer => customer.Email.EmailAddress == email);
        Include(customer => customer.Books);
    }
}
