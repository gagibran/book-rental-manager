namespace BookRentalManager.Application.Dtos;

public sealed record PatchAuthorBooksDto(IEnumerable<Guid> BookIds);
