namespace BookRentalManager.Application.Books.Commands;

public sealed class DeleteBookByIdCommand : ICommand
{
    public Guid Id { get; }

    public DeleteBookByIdCommand(Guid id)
    {
        Id = id;
    }
}
