namespace BookRentalManager.Application.Customers.Commands;

public sealed class DeleteCustomerByIdCommand : ICommand
{
    public Guid Id { get; }

    public DeleteCustomerByIdCommand(Guid id)
    {
        Id = id;
    }
}
