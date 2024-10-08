namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class CreateCustomerCommandHandler(IRepository<Customer> customerRepository)
    : IRequestHandler<CreateCustomerCommand, CustomerCreatedDto>
{
    public async Task<Result<CustomerCreatedDto>> HandleAsync(
        CreateCustomerCommand createCustomerCommand,
        CancellationToken cancellationToken)
    {
        var customerByEmailWithBooksSpecification = new CustomerByEmailWithBooksSpecification(createCustomerCommand.Email);
        Customer? existingCustomerWithEmail = await customerRepository.GetFirstOrDefaultBySpecificationAsync(customerByEmailWithBooksSpecification, cancellationToken);
        Result existingCustomerWithEmailResult = Result.Success();
        if (existingCustomerWithEmail is not null)
        {
            existingCustomerWithEmailResult = Result.Fail(
                "customerEmailAlreadyExists",
                $"A customer with the email '{existingCustomerWithEmail.Email}' already exists.");
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
        await customerRepository.CreateAsync(newCustomer, cancellationToken);
        return Result.Success(new CustomerCreatedDto(newCustomer));
    }
}
