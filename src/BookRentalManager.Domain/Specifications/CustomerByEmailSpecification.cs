namespace BookRentalManager.Domain.Specifications;

public sealed class CustomerByEmailSpecification : Specification<Customer>
{
    private readonly string _email;

    public CustomerByEmailSpecification(string email)
    {
        _email = email;
    }

    public override Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.Email.EmailAddress == _email;
    }
}
