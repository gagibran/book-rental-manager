namespace BookRentalManager.Application.Customers.Commands;

public sealed class ChangeCustomerBooksByBookIdsCommand : IRequest
{
    public Guid Id { get; }
    public bool IsReturn { get; }
    public JsonPatchDocument<ChangeCustomerBooksByBookIdsDto> ChangeCustomerBooksByBookIdsDtoPatchDocument { get; }

    public ChangeCustomerBooksByBookIdsCommand(
        Guid id,
        JsonPatchDocument<ChangeCustomerBooksByBookIdsDto> changeCustomerBooksByBookIdsDtoPatchDocument,
        bool isReturn)
    {
        Id = id;
        IsReturn = isReturn;
        ChangeCustomerBooksByBookIdsDtoPatchDocument = changeCustomerBooksByBookIdsDtoPatchDocument;
    }
}
