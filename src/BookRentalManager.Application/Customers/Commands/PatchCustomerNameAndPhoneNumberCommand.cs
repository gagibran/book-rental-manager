namespace BookRentalManager.Application.Customers.Commands;

public sealed class PatchCustomerNameAndPhoneNumberCommand : ICommand
{
    public Guid Id { get; }
    public JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto> PatchCustomerNameAndPhoneNumberDtoPatchDocument { get; }

    public PatchCustomerNameAndPhoneNumberCommand(Guid id, JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto> patchCustomerNameAndPhoneNumberDtoPatchDocument)
    {
        Id = id;
        PatchCustomerNameAndPhoneNumberDtoPatchDocument = patchCustomerNameAndPhoneNumberDtoPatchDocument;
    }
}
