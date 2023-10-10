namespace BookRentalManager.Application.Authors.CommandHandlers;

internal sealed class PatchAuthorBooksCommandHandler : IRequestHandler<PatchAuthorBooksCommand>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IRepository<Book> _bookRepository;

    public PatchAuthorBooksCommandHandler(IRepository<Author> authorRepository, IRepository<Book> bookRepository)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
    }

    public async Task<Result> HandleAsync(PatchAuthorBooksCommand patchAuthorBooksCommand, CancellationToken cancellationToken)
    {
        var authorByIdWithBooksSpecification = new AuthorByIdWithBooksSpecification(patchAuthorBooksCommand.Id);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdWithBooksSpecification);
        if (author is null)
        {
            return Result.Fail("authorId", $"No author with the ID of '{patchAuthorBooksCommand.Id}' was found.");
        }
        var patchAuthorBooksDto = new PatchAuthorBooksDto(new List<Guid>());
        Result patchAppliedResult = patchAuthorBooksCommand.PatchAuthorBooksDtoPatchDocument.ApplyTo(
            patchAuthorBooksDto,
            "replace",
            "remove");
        if (!patchAppliedResult.IsSuccess)
        {
            return patchAppliedResult;
        }
        var booksByIdsSpecification = new BooksByIdsSpecification(patchAuthorBooksDto.BookIds);
        IReadOnlyList<Book> booksToAdd = await _bookRepository.GetAllBySpecificationAsync(booksByIdsSpecification, cancellationToken);
        if (booksToAdd.Count() != patchAuthorBooksDto.BookIds.Count())
        {
            return Result.Fail("bookIds", "Could not find some of the books for the provided IDs.");
        }
        foreach (Book bookToAdd in booksToAdd)
        {
            Result addBookResult = author!.AddBook(bookToAdd);
            if (!addBookResult.IsSuccess)
            {
                return addBookResult;
            }
        }
        await _authorRepository.UpdateAsync(author!, cancellationToken);
        return Result.Success();
    }
}
