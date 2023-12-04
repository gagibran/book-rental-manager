namespace BookRentalManager.Application.Dtos;

public sealed class PatchAuthorBooksDto(IEnumerable<Guid> bookIds)
{
    public IEnumerable<Guid> BookIds { get; set; } = bookIds;
}
