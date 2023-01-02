namespace BookRentalManager.Application.CustomerCqrs.Queries;

public sealed class GetAllCustomersQuery : IQuery<IReadOnlyList<GetCustomerDto>>
{
}
