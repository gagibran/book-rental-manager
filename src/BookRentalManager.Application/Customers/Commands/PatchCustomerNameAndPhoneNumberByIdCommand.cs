namespace BookRentalManager.Application.Customers.Commands;

public sealed class PatchCustomerNameAndPhoneNumberByIdCommand(Guid id, JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto> patchCustomerNameAndPhoneNumberDtoPatchDocument)
    : IRequest
{
    public Guid Id { get; } = id;
    public JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto> PatchCustomerNameAndPhoneNumberDtoPatchDocument { get; } = patchCustomerNameAndPhoneNumberDtoPatchDocument;
}
