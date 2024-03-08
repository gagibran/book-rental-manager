namespace BookRentalManager.Application.Customers.Queries;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<GetCustomerDto>;
