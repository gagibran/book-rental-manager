namespace BookRentalManager.Application.Authors.CommandHandlers;

internal sealed class DeleteAuthorByIdCommandHandler(IRepository<Author> authorRepository) : IRequestHandler<DeleteAuthorByIdCommand>
{
    public async Task<Result> HandleAsync(DeleteAuthorByIdCommand deleteAuthorByIdCommand, CancellationToken cancellationToken)
    {
        var authorByIdWithBooksSpecification = new AuthorByIdWithBooksSpecification(deleteAuthorByIdCommand.Id);
        Author? author = await authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdWithBooksSpecification, cancellationToken);
        if (author is null)
        {
            return Result.Fail(RequestErrors.IdNotFoundError, $"No author with the ID of '{deleteAuthorByIdCommand.Id}' was found.");
        }
        if (author.Books.Any())
        {
            return Result.Fail("authorHasBooks", "This author has books. Please, delete the books before deleting the author.");
        }
        await authorRepository.DeleteAsync(author, cancellationToken);
        return Result.Success();
    }
}
