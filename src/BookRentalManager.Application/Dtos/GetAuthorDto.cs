namespace BookRentalManager.Application.Dtos;

public sealed class GetAuthorDto(Guid id, FullName fullName, IReadOnlyList<GetBookFromAuthorDto> books) : IdentifiableDto(id)
{
    public string FullName { get; } = fullName.ToString();
    public IReadOnlyList<GetBookFromAuthorDto> Books { get; } = books;

    public GetAuthorDto(Author author) : this(
        author.Id,
        author.FullName,
        author
            .Books
            .Select(book => new GetBookFromAuthorDto(book))
            .ToList()
            .AsReadOnly())
    {
    }
}
