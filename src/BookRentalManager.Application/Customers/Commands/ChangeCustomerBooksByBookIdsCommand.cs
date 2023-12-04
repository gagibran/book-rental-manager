namespace BookRentalManager.Application.Customers.Commands;

public sealed class ChangeCustomerBooksByBookIdsCommand(
    Guid id,
    JsonPatchDocument<ChangeCustomerBooksByBookIdsDto> changeCustomerBooksByBookIdsDtoPatchDocument,
    bool isReturn)
    : IRequest
{
    public Guid Id { get; } = id;
    public bool IsReturn { get; } = isReturn;
    public JsonPatchDocument<ChangeCustomerBooksByBookIdsDto> ChangeCustomerBooksByBookIdsDtoPatchDocument { get; } = changeCustomerBooksByBookIdsDtoPatchDocument;
}
