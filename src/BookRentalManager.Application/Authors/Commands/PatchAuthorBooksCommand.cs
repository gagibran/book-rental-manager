namespace BookRentalManager.Application.Authors.Commands;

public sealed record PatchAuthorBooksCommand(Guid Id, JsonPatchDocument<PatchAuthorBooksDto> PatchAuthorBooksDtoPatchDocument) : IRequest;
