namespace BookRentalManager.Application.Authors.Commands;

public sealed class DeleteAuthorByIdCommand : IRequest
{
    public Guid Id { get; }

    public DeleteAuthorByIdCommand(Guid id)
    {
        Id = id;
    }
}
