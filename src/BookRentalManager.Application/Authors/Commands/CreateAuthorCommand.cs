namespace BookRentalManager.Application.Authors.Commands;

public sealed class CreateAuthorCommand : ICommand<AuthorCreatedDto>
{
    public string FirstName { get; }
    public string LastName { get; }
    public IReadOnlyList<CreateBookForAuthorDto> Books { get; }

    public CreateAuthorCommand(string firstName, string lastName, IReadOnlyList<CreateBookForAuthorDto> books)
    {
        FirstName = firstName;
        LastName = lastName;
        Books = books;
    }
}
