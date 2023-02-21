namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorByIdQueryHandler : IQueryHandler<GetAuthorByIdQuery, GetAuthorDto>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Author, GetAuthorDto> _authorToGetAuthorDtoMapper;

    public GetAuthorByIdQueryHandler(
        IRepository<Author> authorRepository,
        IMapper<Author, GetAuthorDto> authorToGetAuthorDtoMapper)
    {
        _authorRepository = authorRepository;
        _authorToGetAuthorDtoMapper = authorToGetAuthorDtoMapper;
    }

    public async Task<Result<GetAuthorDto>> HandleAsync(GetAuthorByIdQuery getAuthorByIdQuery, CancellationToken cancellationToken)
    {
        var authorByIdSpecification = new AuthorByIdSpecification(getAuthorByIdQuery.Id);
        var author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdSpecification, cancellationToken);
        if (author is null)
        {
            return Result.Fail<GetAuthorDto>("authorId", $"No author with the ID of '{getAuthorByIdQuery.Id}' was found.");
        }
        return Result.Success<GetAuthorDto>(_authorToGetAuthorDtoMapper.Map(author));
    }
}
