namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class PatchCustomerNameAndPhoneNumberByIdCommandHandler : ICommandHandler<PatchCustomerNameAndPhoneNumberByIdCommand>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, PatchCustomerNameAndPhoneNumberDto> _customerToPatchCustomerNameAndPhoneNumberDtoMapper;

    public PatchCustomerNameAndPhoneNumberByIdCommandHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, PatchCustomerNameAndPhoneNumberDto> customerToPatchCustomerNameAndPhoneNumberDtoMapper)
    {
        _customerRepository = customerRepository;
        _customerToPatchCustomerNameAndPhoneNumberDtoMapper = customerToPatchCustomerNameAndPhoneNumberDtoMapper;
    }

    public async Task<Result> HandleAsync(PatchCustomerNameAndPhoneNumberByIdCommand patchCustomerNameAndPhoneNumberByIdCommand, CancellationToken cancellationToken)
    {
        var customerByIdWithBooksSpecification = new CustomerByIdWithBooksSpecification(patchCustomerNameAndPhoneNumberByIdCommand.Id);
        Customer? customer = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(customerByIdWithBooksSpecification, cancellationToken);
        if (customer is null)
        {
            return Result.Fail("customerId", $"No customer with the ID of '{patchCustomerNameAndPhoneNumberByIdCommand.Id}' was found.");
        }
        PatchCustomerNameAndPhoneNumberDto patchCustomerNameAndPhoneNumberDto = _customerToPatchCustomerNameAndPhoneNumberDtoMapper.Map(customer);
        Result patchAppliedResult = JsonPatchDocumentExtensions.ApplyTo(
            patchCustomerNameAndPhoneNumberByIdCommand.PatchCustomerNameAndPhoneNumberDtoPatchDocument,
            patchCustomerNameAndPhoneNumberDto,
            "add",
            "remove");
        Result updateFullNameAndPhoneNumberResult = customer.UpdateFullNameAndPhoneNumber(
            patchCustomerNameAndPhoneNumberDto.FirstName,
            patchCustomerNameAndPhoneNumberDto.LastName,
            patchCustomerNameAndPhoneNumberDto.AreaCode,
            patchCustomerNameAndPhoneNumberDto.PrefixAndLineNumber);
        Result combinedResult = Result.Combine(patchAppliedResult, updateFullNameAndPhoneNumberResult);
        if (!combinedResult.IsSuccess)
        {
            return combinedResult;
        }
        await _customerRepository.UpdateAsync(customer, cancellationToken);
        return Result.Success();
    }
}
