namespace BookRentalManager.Application.Authors.Commands;

public sealed class PatchAuthorBooksCommand(Guid id, JsonPatchDocument<PatchAuthorBooksDto> patchAuthorBooksDtoPatchDocument)
    : IRequest
{
    public Guid Id { get; } = id;
    public JsonPatchDocument<PatchAuthorBooksDto> PatchAuthorBooksDtoPatchDocument { get; } = patchAuthorBooksDtoPatchDocument;
}
