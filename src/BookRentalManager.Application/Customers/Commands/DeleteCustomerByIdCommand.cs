namespace BookRentalManager.Application.Customers.Commands;

public sealed record DeleteCustomerByIdCommand(Guid Id) : IRequest;
