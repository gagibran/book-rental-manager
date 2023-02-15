namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CustomerCreatedDto>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, CustomerCreatedDto> _customerToCustomerCreatedDtoMapper;

    public CreateCustomerCommandHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, CustomerCreatedDto> customerToCustomerCreatedDtoMapper)
    {
        _customerRepository = customerRepository;
        _customerToCustomerCreatedDtoMapper = customerToCustomerCreatedDtoMapper;
    }

    public async Task<Result<CustomerCreatedDto>> HandleAsync(
        CreateCustomerCommand createCustomerCommand,
        CancellationToken cancellationToken)
    {
        var customerByEmailSpecification = new CustomerByEmailSpecification(createCustomerCommand.Email);
        Customer? existingCustomerWithEmail = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(customerByEmailSpecification);
        Result existingCustomerWithEmailResult = Result.Success();
        if (existingCustomerWithEmail is not null)
        {
            existingCustomerWithEmailResult = Result.Fail(
                "customerEmailAlreadyExists",
                $"A customer with the email '{existingCustomerWithEmail.Email.EmailAddress}' already exists.");
        }
        Result<FullName> fullNameResult = FullName.Create(createCustomerCommand.FirstName, createCustomerCommand.LastName);
        Result<Email> emailResult = Email.Create(createCustomerCommand.Email);
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(
            createCustomerCommand.PhoneNumber.AreaCode,
            createCustomerCommand.PhoneNumber.PrefixAndLineNumber);
        Result combinedResults = Result.Combine(existingCustomerWithEmailResult, fullNameResult, emailResult, phoneNumberResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<CustomerCreatedDto>(combinedResults.ErrorType, combinedResults.ErrorMessage);
        }
        var newCustomer = new Customer(fullNameResult.Value!, emailResult.Value!, phoneNumberResult.Value!);
        await _customerRepository.CreateAsync(newCustomer, cancellationToken);
        return Result.Success(_customerToCustomerCreatedDtoMapper.Map(newCustomer));
    }
}
