namespace BookRentalManager.Application.Customers.Commands;

public sealed class AddNewCustomerCommand : ICommand
{
    public Customer Customer { get; }

    public AddNewCustomerCommand(Customer customer)
    {
        Customer = customer;
    }
}
