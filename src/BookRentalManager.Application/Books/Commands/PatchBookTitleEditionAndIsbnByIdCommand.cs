namespace BookRentalManager.Application.Books.Commands;

public sealed class PatchBookTitleEditionAndIsbnByIdCommand(
    Guid id,
    JsonPatchDocument<PatchBookTitleEditionAndIsbnByIdDto> patchBookTitleEditionAndIsbnByIdDtoPatchDocument)
    : IRequest
{
    public Guid Id { get; } = id;
    public JsonPatchDocument<PatchBookTitleEditionAndIsbnByIdDto> PatchBookTitleEditionAndIsbnByIdDtoPatchDocument { get; } = patchBookTitleEditionAndIsbnByIdDtoPatchDocument;
}
