using BookRentalManager.Application.Interfaces;

namespace BookRentalManager.Application.CustomerCqrs.Queries;

public sealed class GetCustomersQuery : GetAllEntitiesQuery, IQuery<IReadOnlyList<GetCustomerDto>>
{
    public GetCustomersQuery(int pageIndex, int totalItemsPerPage)
        : base(pageIndex, totalItemsPerPage)
    {
    }
}
