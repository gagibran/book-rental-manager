namespace BookRentalManager.Application.Customers.Commands;

public sealed record ChangeCustomerBooksByBookIdsCommand(
    Guid Id,
    JsonPatchDocument<ChangeCustomerBooksByBookIdsDto> ChangeCustomerBooksByBookIdsDtoPatchDocument,
    bool IsReturn)
    : IRequest;
