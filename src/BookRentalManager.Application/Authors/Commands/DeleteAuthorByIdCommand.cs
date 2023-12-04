namespace BookRentalManager.Application.Authors.Commands;

public sealed class DeleteAuthorByIdCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
