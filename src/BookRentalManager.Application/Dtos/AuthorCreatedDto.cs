namespace BookRentalManager.Application.Dtos;

public sealed class AuthorCreatedDto(Guid id, string firstName, string lastName) : IdentifiableDto(id)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;

    public AuthorCreatedDto(Author author) : this(author.Id, author.FullName.FirstName, author.FullName.LastName)
    {
    }
}
