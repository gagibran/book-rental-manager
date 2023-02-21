namespace BookRentalManager.Application.Dtos;

public sealed class AuthorCreatedDto
{
    public Guid Id { get; }

    public string FullName { get; }

    public AuthorCreatedDto(Guid id, string fullName)
    {
        Id = id;
        FullName = fullName;
    }
}
