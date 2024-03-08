namespace BookRentalManager.Application.Dtos;

public sealed record GetAuthorDto(Guid Id, string FullName, IReadOnlyList<GetBookFromAuthorDto> Books) : IdentifiableDto(Id)
{
    public GetAuthorDto(Author author) : this(
        author.Id,
        author.FullName.ToString(),
        author.Books
            .Select(book => new GetBookFromAuthorDto(book))
            .ToList()
            .AsReadOnly())
    {
    }
}
