namespace BookRentalManager.Application.Dtos;

public sealed class PatchAuthorBooksDto
{
    public IEnumerable<Guid> BookIds { get; set; }

    public PatchAuthorBooksDto(IEnumerable<Guid> bookIds)
    {
        BookIds = bookIds;
    }
}
