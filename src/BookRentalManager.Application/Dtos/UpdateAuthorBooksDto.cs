namespace BookRentalManager.Application.Dtos;

public sealed class UpdateAuthorBooksDto
{
    public IReadOnlyList<Guid> BookIds { get; }

    public UpdateAuthorBooksDto(IReadOnlyList<Guid> bookIds)
    {
        BookIds = bookIds;
    }
}
