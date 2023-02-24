namespace BookRentalManager.Application.Dtos;

public sealed class AuthorCreatedDto
{
    public Guid Id { get; }

    public string FirstName { get; }
    public string LastName { get; }

    public AuthorCreatedDto(Guid id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }
}
