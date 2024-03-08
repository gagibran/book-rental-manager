namespace BookRentalManager.Application.Books.Commands;

public sealed record PatchBookTitleEditionAndIsbnByIdCommand(
    Guid Id,
    JsonPatchDocument<PatchBookTitleEditionAndIsbnByIdDto> PatchBookTitleEditionAndIsbnByIdDtoPatchDocument)
    : IRequest;
