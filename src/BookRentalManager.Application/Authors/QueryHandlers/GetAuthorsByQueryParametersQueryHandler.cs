namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorsByQueryParametersQueryHandler
    : IQueryHandler<GetAuthorsByQueryParametersQuery, PaginatedList<GetAuthorDto>>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Author, GetAuthorDto> _authorToGetAuthorDtoMapper;
    private readonly IMapper<AuthorSortParameters, string> _authorSortParametersMapper;

    public GetAuthorsByQueryParametersQueryHandler(
        IRepository<Author> authorRepository,
        IMapper<Author, GetAuthorDto> authorToGetAuthorDtoMapper,
        IMapper<AuthorSortParameters, string> authorSortParametersMapper)
    {
        _authorRepository = authorRepository;
        _authorToGetAuthorDtoMapper = authorToGetAuthorDtoMapper;
        _authorSortParametersMapper = authorSortParametersMapper;
    }

    public async Task<Result<PaginatedList<GetAuthorDto>>> HandleAsync(
        GetAuthorsByQueryParametersQuery getAuthorsByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        string convertedSorParameters = _authorSortParametersMapper.Map(
            new AuthorSortParameters(getAuthorsByQueryParametersQuery.SortParameters));
        var authorsBySearchParameterSpecification = new AuthorsBySearchParameterSpecification(
            getAuthorsByQueryParametersQuery.SearchParameter,
            convertedSorParameters);
        PaginatedList<Author> authors = await _authorRepository.GetAllBySpecificationAsync(
            getAuthorsByQueryParametersQuery.PageIndex,
            getAuthorsByQueryParametersQuery.PageSize,
            authorsBySearchParameterSpecification,
            cancellationToken);
        List<GetAuthorDto> getAuthorDtos = (from author in authors
                                            select _authorToGetAuthorDtoMapper.Map(author)).ToList();
        var paginatedGetAuthorDtos = new PaginatedList<GetAuthorDto>(
            getAuthorDtos,
            authors.TotalAmountOfItems,
            authors.TotalAmountOfPages,
            authors.PageIndex,
            authors.PageSize);
        return Result.Success<PaginatedList<GetAuthorDto>>(paginatedGetAuthorDtos);
    }
}
