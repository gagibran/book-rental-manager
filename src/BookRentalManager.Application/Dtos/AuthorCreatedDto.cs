namespace BookRentalManager.Application.Dtos;

public sealed class AuthorCreatedDto : SingleResourceBaseDto
{
    public string FirstName { get; }
    public string LastName { get; }

    public AuthorCreatedDto(Guid id, string firstName, string lastName) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
