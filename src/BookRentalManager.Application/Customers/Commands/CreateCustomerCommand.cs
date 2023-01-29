namespace BookRentalManager.Application.Customers.Commands;

public sealed class CreateCustomerCommand : ICommand<CustomerCreatedDto>
{
    public CreateCustomerDto CreateCustomerDto { get; }

    public CreateCustomerCommand(CreateCustomerDto createCustomerDto)
    {
        CreateCustomerDto = createCustomerDto;
    }
}
