namespace BookRentalManager.Application.Customers.Commands;

public sealed class CreateNewCustomerCommand : ICommand
{
    public Customer Customer { get; }

    public CreateNewCustomerCommand(Customer customer)
    {
        Customer = customer;
    }
}
