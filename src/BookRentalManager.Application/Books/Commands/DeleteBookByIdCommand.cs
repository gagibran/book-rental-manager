namespace BookRentalManager.Application.Books.Commands;

public sealed class DeleteBookByIdCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
