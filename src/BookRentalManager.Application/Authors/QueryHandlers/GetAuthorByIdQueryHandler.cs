namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorByIdQueryHandler(IRepository<Author> authorRepository) : IRequestHandler<GetAuthorByIdQuery, GetAuthorDto>
{
    public async Task<Result<GetAuthorDto>> HandleAsync(GetAuthorByIdQuery getAuthorByIdQuery, CancellationToken cancellationToken)
    {
        var authorByIdWithBooksSpecification = new AuthorByIdWithBooksSpecification(getAuthorByIdQuery.Id);
        var author = await authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdWithBooksSpecification, cancellationToken);
        if (author is null)
        {
            return Result.Fail<GetAuthorDto>(RequestErrors.IdNotFoundError, $"No author with the ID of '{getAuthorByIdQuery.Id}' was found.");
        }
        return Result.Success(new GetAuthorDto(author));
    }
}
