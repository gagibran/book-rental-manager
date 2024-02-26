namespace BookRentalManager.Application.Authors.CommandHandlers;

internal sealed class PatchAuthorBooksCommandHandler(IRepository<Author> authorRepository, IRepository<Book> bookRepository)
    : IRequestHandler<PatchAuthorBooksCommand>
{
    public async Task<Result> HandleAsync(PatchAuthorBooksCommand patchAuthorBooksCommand, CancellationToken cancellationToken)
    {
        var authorByIdWithBooksSpecification = new AuthorByIdWithBooksSpecification(patchAuthorBooksCommand.Id);
        Author? author = await authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdWithBooksSpecification, cancellationToken);
        if (author is null)
        {
            return Result.Fail(RequestErrors.IdNotFoundError, $"No author with the ID of '{patchAuthorBooksCommand.Id}' was found.");
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
        IReadOnlyList<Book> booksToAdd = await bookRepository.GetAllBySpecificationAsync(booksByIdsSpecification, cancellationToken);
        if (booksToAdd.Count != patchAuthorBooksDto.BookIds.Count())
        {
            return Result.Fail("bookIds", "Could not find some of the books for the provided IDs.");
        }
        foreach (Book bookToAdd in booksToAdd)
        {
            author.AddBook(bookToAdd);
        }
        await authorRepository.UpdateAsync(author!, cancellationToken);
        return Result.Success();
    }
}
