namespace BookRentalManager.Application.Customers.Commands;

public sealed class DeleteCustomerByIdCommand : IRequest
{
    public Guid Id { get; }

    public DeleteCustomerByIdCommand(Guid id)
    {
        Id = id;
    }
}
