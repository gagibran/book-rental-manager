namespace BookRentalManager.Application.Customers.Queries;

public sealed class GetCustomerByIdQuery(Guid id) : IRequest<GetCustomerDto>
{
    public Guid Id { get; } = id;
}
