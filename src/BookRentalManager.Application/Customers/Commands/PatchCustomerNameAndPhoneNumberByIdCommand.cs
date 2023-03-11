namespace BookRentalManager.Application.Customers.Commands;

public sealed class PatchCustomerNameAndPhoneNumberByIdCommand : ICommand
{
    public Guid Id { get; }
    public JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto> PatchCustomerNameAndPhoneNumberDtoPatchDocument { get; }

    public PatchCustomerNameAndPhoneNumberByIdCommand(Guid id, JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto> patchCustomerNameAndPhoneNumberDtoPatchDocument)
    {
        Id = id;
        PatchCustomerNameAndPhoneNumberDtoPatchDocument = patchCustomerNameAndPhoneNumberDtoPatchDocument;
    }
}
