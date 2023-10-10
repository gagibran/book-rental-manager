namespace BookRentalManager.Application.Books.Commands;

public sealed class PatchBookTitleEditionAndIsbnByIdCommand : IRequest
{
    public Guid Id { get; }
    public JsonPatchDocument<PatchBookTitleEditionAndIsbnByIdDto> PatchBookTitleEditionAndIsbnByIdDtoPatchDocument { get; }

    public PatchBookTitleEditionAndIsbnByIdCommand(
        Guid id,
        JsonPatchDocument<PatchBookTitleEditionAndIsbnByIdDto> patchBookTitleEditionAndIsbnByIdDtoPatchDocument)
    {
        Id = id;
        PatchBookTitleEditionAndIsbnByIdDtoPatchDocument = patchBookTitleEditionAndIsbnByIdDtoPatchDocument;
    }
}
