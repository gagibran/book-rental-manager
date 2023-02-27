namespace BookRentalManager.Application.Customers.Commands;

public sealed class ReturnBooksByBookIdsCommand : ICommand
{
    public Guid Id { get; }
    public JsonPatchDocument<ReturnCustomerBookByIdDto> ReturnCustomerBookByIdDtoPatchDocument { get; }

    public ReturnBooksByBookIdsCommand(Guid id, JsonPatchDocument<ReturnCustomerBookByIdDto> returnCustomerBookByIdDtoPatchDocument)
    {
        Id = id;
        ReturnCustomerBookByIdDtoPatchDocument = returnCustomerBookByIdDtoPatchDocument;
    }
}
