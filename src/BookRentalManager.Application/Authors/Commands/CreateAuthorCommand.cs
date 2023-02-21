namespace BookRentalManager.Application.Authors.Commands;

public sealed class CreateAuthorCommand : ICommand<AuthorCreatedDto>
{
    public string FirstName { get; }
    public string LastName { get; }

    public CreateAuthorCommand(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
