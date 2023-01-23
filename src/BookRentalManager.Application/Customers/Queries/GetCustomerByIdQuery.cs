namespace BookRentalManager.Application.Customers.Queries;

public sealed class GetCustomerByIdQuery : IQuery<GetCustomerDto>
{
    public Guid Id { get; }

    public GetCustomerByIdQuery(Guid id)
    {
        Id = id;
    }
}
