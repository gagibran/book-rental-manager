using BookRentalManager.Application.Interfaces;

namespace BookRentalManager.Application.CustomerCqrs.Queries;

public sealed class GetCustomersQuery : GetAllEntitiesQuery, IQuery<IReadOnlyList<GetCustomerDto>>
{
    public string Email { get; }

    public GetCustomersQuery(int pageIndex, int totalItemsPerPage, string email)
        : base(pageIndex, totalItemsPerPage)
    {
        Email = email;
    }
}
