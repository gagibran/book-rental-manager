namespace BookRentalManager.Application.Customers.Queries;

public sealed class GetCustomerWithBooksByIdQuery : IQuery<GetCustomerDto>
{
    public Guid Id { get; }

    public GetCustomerWithBooksByIdQuery(Guid id)
    {
        Id = id;
    }
}
