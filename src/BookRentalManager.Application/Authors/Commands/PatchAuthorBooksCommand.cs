namespace BookRentalManager.Application.Authors.Commands;

public sealed class PatchAuthorBooksCommand : IRequest
{
    public Guid Id { get; }
    public JsonPatchDocument<PatchAuthorBooksDto> PatchAuthorBooksDtoPatchDocument { get; }

    public PatchAuthorBooksCommand(Guid id, JsonPatchDocument<PatchAuthorBooksDto> patchAuthorBooksDtoPatchDocument)
    {
        Id = id;
        PatchAuthorBooksDtoPatchDocument = patchAuthorBooksDtoPatchDocument;
    }
}
