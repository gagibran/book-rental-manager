namespace BookRentalManager.Application.CustomerCqrs.Commands;

public sealed class AddNewCustomerCommand : ICommand
{
    public Customer Customer { get; }

    public AddNewCustomerCommand(Customer customer)
    {
        Customer = customer;
    }
}
