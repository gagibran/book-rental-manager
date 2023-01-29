namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorsByQueryParametersQueryHandler
    : IQueryHandler<GetAuthorsByQueryParametersQuery, IReadOnlyList<GetAuthorDto>>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Author, GetAuthorDto> _getAuthorDtoMapper;

    public GetAuthorsByQueryParametersQueryHandler(
        IRepository<Author> authorRepository,
        IMapper<Author, GetAuthorDto> getAuthorDtoMapper)
    {
        _authorRepository = authorRepository;
        _getAuthorDtoMapper = getAuthorDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetAuthorDto>>> HandleAsync(
        GetAuthorsByQueryParametersQuery getAuthorsByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Author> authors = await _authorRepository.GetAllBySpecificationAsync(
            getAuthorsByQueryParametersQuery.PageIndex,
            getAuthorsByQueryParametersQuery.TotalItemsPerPage,
            new AuthorsBySearchParameterSpecification(getAuthorsByQueryParametersQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetAuthorDto> getAuthorDtos = (from author in authors
                                                     select _getAuthorDtoMapper.Map(author)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetAuthorDto>>(getAuthorDtos);
    }
}
