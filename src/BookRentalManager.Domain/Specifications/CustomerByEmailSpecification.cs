namespace BookRentalManager.Domain.Specifications;

public sealed class CustomerByEmailSpecification : Specification<Customer>
{
    private readonly Email _email;

    public CustomerByEmailSpecification(Email email)
    {
        _email = email;
    }

    public override Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.Email == _email;
    }
}
