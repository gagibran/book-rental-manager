using BookRentalManager.Application.Interfaces;

namespace BookRentalManager.Application.CustomerCqrs.Queries;

public sealed class GetCustomersWithQueryParameterQuery : GetEntitiesQuery, IQuery<IReadOnlyList<GetCustomerDto>>
{
    public string QueryParameter { get; }

    public GetCustomersWithQueryParameterQuery(int pageIndex, int totalItemsPerPage, string queryParameter)
        : base(pageIndex, totalItemsPerPage)
    {
        QueryParameter = queryParameter;
    }
}
