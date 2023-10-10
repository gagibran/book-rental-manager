namespace BookRentalManager.Application.Books.Commands;

public sealed class DeleteBookByIdCommand : IRequest
{
    public Guid Id { get; }

    public DeleteBookByIdCommand(Guid id)
    {
        Id = id;
    }
}
