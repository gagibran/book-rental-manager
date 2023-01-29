namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorsByQueryParameterQueryHandler
    : IQueryHandler<GetAuthorsByQueryParameterQuery, IReadOnlyList<GetAuthorDto>>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Author, GetAuthorDto> _getAuthorDtoMapper;

    public GetAuthorsByQueryParameterQueryHandler(
        IRepository<Author> authorRepository,
        IMapper<Author, GetAuthorDto> getAuthorDtoMapper)
    {
        _authorRepository = authorRepository;
        _getAuthorDtoMapper = getAuthorDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetAuthorDto>>> HandleAsync(
        GetAuthorsByQueryParameterQuery getAuthorsByQueryParameterQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Author> authors = await _authorRepository.GetAllBySpecificationAsync(
            getAuthorsByQueryParameterQuery.PageIndex,
            getAuthorsByQueryParameterQuery.TotalItemsPerPage,
            new AuthorsBySearchParameterSpecification(getAuthorsByQueryParameterQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetAuthorDto> getAuthorDtos = (from author in authors
                                                     select _getAuthorDtoMapper.Map(author)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetAuthorDto>>(getAuthorDtos);
    }
}
