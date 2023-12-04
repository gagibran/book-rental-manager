namespace BookRentalManager.Application.Authors.Commands;

public sealed class CreateAuthorCommand(string firstName, string lastName) : IRequest<AuthorCreatedDto>
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
}
