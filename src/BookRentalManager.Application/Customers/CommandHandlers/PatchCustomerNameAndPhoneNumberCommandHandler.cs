using BookRentalManager.Application.Extensions;

namespace BookRentalManager.Application.Customers.CommandHandlers;

internal sealed class PatchCustomerNameAndPhoneNumberCommandHandler : ICommandHandler<PatchCustomerNameAndPhoneNumberCommand>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper<Customer, PatchCustomerNameAndPhoneNumberDto> _customerToPatchCustomerNameAndPhoneNumberDtoMapper;

    public PatchCustomerNameAndPhoneNumberCommandHandler(
        IRepository<Customer> customerRepository,
        IMapper<Customer, PatchCustomerNameAndPhoneNumberDto> customerToPatchCustomerNameAndPhoneNumberDtoMapper)
    {
        _customerRepository = customerRepository;
        _customerToPatchCustomerNameAndPhoneNumberDtoMapper = customerToPatchCustomerNameAndPhoneNumberDtoMapper;
    }

    public async Task<Result> HandleAsync(PatchCustomerNameAndPhoneNumberCommand patchCustomerNameAndPhoneNumberCommand, CancellationToken cancellationToken)
    {
        var customerByIdWithBooksSpecification = new CustomerByIdWithBooksSpecification(patchCustomerNameAndPhoneNumberCommand.Id);
        Customer? customer = await _customerRepository.GetFirstOrDefaultBySpecificationAsync(customerByIdWithBooksSpecification, cancellationToken);
        if (customer is null)
        {
            return Result.Fail("customerId", $"No customer with the ID of '{patchCustomerNameAndPhoneNumberCommand.Id}' was found.");
        }
        PatchCustomerNameAndPhoneNumberDto patchCustomerNameAndPhoneNumberDto = _customerToPatchCustomerNameAndPhoneNumberDtoMapper.Map(customer);
        Result patchAppliedResult = JsonPatchDocumentExtensions.ApplyTo(patchCustomerNameAndPhoneNumberCommand.PatchCustomerNameAndPhoneNumberDtoPatchDocument, patchCustomerNameAndPhoneNumberDto);
        Result<FullName> fullNameResult = FullName.Create(patchCustomerNameAndPhoneNumberDto.FirstName, patchCustomerNameAndPhoneNumberDto.LastName);
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(
            patchCustomerNameAndPhoneNumberDto.AreaCode,
            patchCustomerNameAndPhoneNumberDto.PrefixAndLineNumber);
        Result combinedResult = Result.Combine(patchAppliedResult, fullNameResult, phoneNumberResult);
        if (!combinedResult.IsSuccess)
        {
            return Result.Fail(combinedResult.ErrorType, combinedResult.ErrorMessage);
        }
        customer.UpdateFullName(fullNameResult.Value!);
        customer.UpdatePhoneNumber(phoneNumberResult.Value!);
        await _customerRepository.UpdateAsync(customer, cancellationToken);
        return Result.Success();
    }
}
