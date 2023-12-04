namespace BookRentalManager.Application.Customers.Commands;

public sealed class DeleteCustomerByIdCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
