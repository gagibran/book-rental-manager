namespace BookRentalManager.Application.CustomerCqrs.Queries;

public sealed class GetCustomerByIdQuery : IQuery<GetCustomerDto>
{
    public Guid Id { get; }

    public GetCustomerByIdQuery(Guid id)
    {
        Id = id;
    }
}
