namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CustomerCreatedDto>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, CustomerCreatedDto> _customerCreatedDtoMapper;

    public CreateCustomerCommandHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, CustomerCreatedDto> customerCreatedDtoMapper)
    {
        _customerRepository = customerRepository;
        _customerCreatedDtoMapper = customerCreatedDtoMapper;
    }

    public async Task<Result<CustomerCreatedDto>> HandleAsync(
        CreateCustomerCommand createCustomerCommand,
        CancellationToken cancellationToken)
    {
        Customer? customerWithEmail = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(
            new CustomerByEmailSpecification(createCustomerCommand.Email));
        if (customerWithEmail is not null)
        {
            return Result.Fail<CustomerCreatedDto>(
                $"A customer with the email '{customerWithEmail.Email.EmailAddress}' already exists.");
        }
        Result<FullName> fullNameResult = FullName.Create(createCustomerCommand.FirstName, createCustomerCommand.LastName);
        Result<Email> emailResult = Email.Create(createCustomerCommand.Email);
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(
            createCustomerCommand.AreaCode,
            createCustomerCommand.PrefixAndLineNumber);
        Result combinedResults = Result.Combine(fullNameResult, emailResult, phoneNumberResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<CustomerCreatedDto>(combinedResults.ErrorMessage);
        }
        var newCustomer = new Customer(fullNameResult.Value!, emailResult.Value!, phoneNumberResult.Value!);
        await _customerRepository.CreateAsync(newCustomer, cancellationToken);
        return Result.Success(_customerCreatedDtoMapper.Map(newCustomer));
    }
}
