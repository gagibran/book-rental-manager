namespace BookRentalManager.Application.Dtos;

public sealed class AuthorCreatedDto
{
    public Guid Id { get; }

    public string FullName { get; }

    public IReadOnlyList<BookCreatedForAuthorDto> Books { get; }

    public AuthorCreatedDto(Guid id, string fullName, IReadOnlyList<BookCreatedForAuthorDto> books)
    {
        Id = id;
        FullName = fullName;
        Books = books;
    }
}
