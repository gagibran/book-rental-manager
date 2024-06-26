namespace BookRentalManager.Application.Dtos;

[method: JsonConstructor]
public sealed record AuthorCreatedDto(Guid Id, string FirstName, string LastName) : IdentifiableDto(Id)
{
    public AuthorCreatedDto(Author author) : this(author.Id, author.FullName.FirstName, author.FullName.LastName)
    {
    }
}
