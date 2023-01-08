namespace BookRentalManager.Application.CustomerCqrs.Queries;

public sealed class GetCustomersQuery : GetEntitiesQuery, IQuery<IReadOnlyList<GetCustomerDto>>
{
    public GetCustomersQuery(int pageIndex, int totalItemsPerPage)
        : base(pageIndex, totalItemsPerPage)
    {
    }
}
