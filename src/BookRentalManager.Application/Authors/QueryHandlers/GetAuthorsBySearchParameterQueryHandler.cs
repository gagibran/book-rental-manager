namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorsBySearchParameterQueryHandler
    : IQueryHandler<GetAuthorsBySearchParameterQuery, IReadOnlyList<GetAuthorDto>>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Author, GetAuthorDto> _getAuthorDtoMapper;

    public GetAuthorsBySearchParameterQueryHandler(
        IRepository<Author> authorRepository,
        IMapper<Author, GetAuthorDto> getAuthorDtoMapper)
    {
        _authorRepository = authorRepository;
        _getAuthorDtoMapper = getAuthorDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetAuthorDto>>> HandleAsync(
        GetAuthorsBySearchParameterQuery getAuthorsBySearchParameterQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Author> authors = await _authorRepository.GetAllBySpecificationAsync(
            getAuthorsBySearchParameterQuery.PageIndex,
            getAuthorsBySearchParameterQuery.TotalItemsPerPage,
            new AuthorsBySearchParameterSpecification(getAuthorsBySearchParameterQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetAuthorDto> getAuthorDtos = (from author in authors
                                                     select _getAuthorDtoMapper.Map(author)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetAuthorDto>>(getAuthorDtos);
    }
}
