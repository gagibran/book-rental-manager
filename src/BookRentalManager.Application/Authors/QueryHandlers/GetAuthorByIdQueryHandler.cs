namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorByIdQueryHandler(
    IRepository<Author> authorRepository,
    IMapper<Author, GetAuthorDto> authorToGetAuthorDtoMapper)
    : IRequestHandler<GetAuthorByIdQuery, GetAuthorDto>
{
    private readonly IRepository<Author> _authorRepository = authorRepository;
    private readonly IMapper<Author, GetAuthorDto> _authorToGetAuthorDtoMapper = authorToGetAuthorDtoMapper;

    public async Task<Result<GetAuthorDto>> HandleAsync(GetAuthorByIdQuery getAuthorByIdQuery, CancellationToken cancellationToken)
    {
        var authorByIdWithBooksSpecification = new AuthorByIdWithBooksSpecification(getAuthorByIdQuery.Id);
        var author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdWithBooksSpecification, cancellationToken);
        if (author is null)
        {
            return Result.Fail<GetAuthorDto>("authorId", $"No author with the ID of '{getAuthorByIdQuery.Id}' was found.");
        }
        return Result.Success(_authorToGetAuthorDtoMapper.Map(author));
    }
}
