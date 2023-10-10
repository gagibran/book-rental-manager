namespace BookRentalManager.Application.Authors.Commands;

public sealed class CreateAuthorCommand : IRequest<AuthorCreatedDto>
{
    public string FirstName { get; }
    public string LastName { get; }

    public CreateAuthorCommand(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
