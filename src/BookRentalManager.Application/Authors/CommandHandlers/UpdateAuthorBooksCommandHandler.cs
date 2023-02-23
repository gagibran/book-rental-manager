namespace BookRentalManager.Application.Authors.CommandHandlers;

internal sealed class UpdateAuthorBooksCommandHandler : ICommandHandler<UpdateAuthorBooksCommand>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IRepository<Book> _bookRepository;

    public UpdateAuthorBooksCommandHandler(IRepository<Author> authorRepository, IRepository<Book> bookRepository)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
    }

    public async Task<Result> HandleAsync(UpdateAuthorBooksCommand updateAuthorBooksCommand, CancellationToken cancellationToken)
    {
        var authorByIdWithBooksSpecification = new AuthorByIdWithBooksSpecification(updateAuthorBooksCommand.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdWithBooksSpecification);
        if (author is null)
        {
            return Result.Fail<GetAuthorDto>(
                "authorId", $"No author with the ID of '{updateAuthorBooksCommand.AuthorId}' was found.");
        }
        var booksByIdsSpecification = new BooksByIdsSpecification(updateAuthorBooksCommand.BookIds);
        IReadOnlyList<Book> booksToAdd = await _bookRepository.GetAllBySpecificationAsync(booksByIdsSpecification, cancellationToken);
        if (!booksToAdd.Select(bookToAdd => bookToAdd.Id).SequenceEqual(updateAuthorBooksCommand.BookIds))
        {
            return Result.Fail("bookIds", "Could not find some of the books for the provided IDs.");
        }
        foreach (Book bookToAdd in booksToAdd)
        {
            Result addBookResult = author!.AddBook(bookToAdd);
            if (!addBookResult.IsSuccess)
            {
                return Result.Fail(addBookResult.ErrorType, addBookResult.ErrorMessage);
            }
        }
        await _authorRepository.UpdateAsync(author!, cancellationToken);
        return Result.Success();
    }
}
