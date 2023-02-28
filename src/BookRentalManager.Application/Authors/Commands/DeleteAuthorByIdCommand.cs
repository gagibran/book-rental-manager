namespace BookRentalManager.Application.Authors.Commands;

public sealed class DeleteAuthorByIdCommand : ICommand
{
    public Guid Id { get; }

    public DeleteAuthorByIdCommand(Guid id)
    {
        Id = id;
    }
}
