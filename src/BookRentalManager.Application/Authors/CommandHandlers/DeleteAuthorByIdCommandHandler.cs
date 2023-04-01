namespace BookRentalManager.Application.Authors.CommandHandlers;

internal sealed class DeleteAuthorByIdCommandHandler : IRequestHandler<DeleteAuthorByIdCommand>
{
    private readonly IRepository<Author> _authorRepository;

    public DeleteAuthorByIdCommandHandler(IRepository<Author> authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result> HandleAsync(DeleteAuthorByIdCommand deleteAuthorByIdCommand, CancellationToken cancellationToken)
    {
        var authorByIdWithBooksSpecification = new AuthorByIdWithBooksSpecification(deleteAuthorByIdCommand.Id);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdWithBooksSpecification, cancellationToken);
        if (author is null)
        {
            return Result.Fail("authorId", $"No author with the ID of '{deleteAuthorByIdCommand.Id}' was found.");
        }
        Result authorHasRentedBooksResult = Result.Success();
        if (author.Books.Any())
        {
            return Result.Fail("authorHasBooks", "This author has books. Please, delete the books before deleting the author.");
        }
        if (!authorHasRentedBooksResult.IsSuccess)
        {
            return authorHasRentedBooksResult;
        }
        await _authorRepository.DeleteAsync(author, cancellationToken);
        return Result.Success();
    }
}
