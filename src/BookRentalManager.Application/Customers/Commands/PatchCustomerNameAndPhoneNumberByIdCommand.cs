namespace BookRentalManager.Application.Customers.Commands;

public sealed record PatchCustomerNameAndPhoneNumberByIdCommand(
    Guid Id,
    JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto> PatchCustomerNameAndPhoneNumberDtoPatchDocument)
    : IRequest;
