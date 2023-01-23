namespace BookRentalManager.Application.Customers.Commands;

public sealed class CreateCustomerCommand : ICommand
{
    public Customer Customer { get; }

    public CreateCustomerCommand(Customer customer)
    {
        Customer = customer;
    }
}
