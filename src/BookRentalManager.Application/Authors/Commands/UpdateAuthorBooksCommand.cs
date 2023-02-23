namespace BookRentalManager.Application.Authors.Commands;

public sealed class UpdateAuthorBooksCommand : ICommand
{
    public Guid AuthorId { get; }
    public IReadOnlyList<Guid> BookIds { get; }

    public UpdateAuthorBooksCommand(Guid authorId, IReadOnlyList<Guid> bookIds)
    {
        AuthorId = authorId;
        BookIds = bookIds;
    }
}
