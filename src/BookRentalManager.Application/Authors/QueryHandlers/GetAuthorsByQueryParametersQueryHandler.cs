namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorsByQueryParametersQueryHandler
    : IQueryHandler<GetAuthorsByQueryParametersQuery, PaginatedList<GetAuthorDto>>
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

    public async Task<Result<PaginatedList<GetAuthorDto>>> HandleAsync(
        GetAuthorsByQueryParametersQuery getAuthorsByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        PaginatedList<Author> authors = await _authorRepository.GetAllBySpecificationAsync(
            getAuthorsByQueryParametersQuery.PageIndex,
            getAuthorsByQueryParametersQuery.TotalAmountOfItemsPerPage,
            new AuthorsBySearchParameterSpecification(getAuthorsByQueryParametersQuery.SearchParameter),
            cancellationToken);
        List<GetAuthorDto> getAuthorDtos = (from author in authors
                                            select _getAuthorDtoMapper.Map(author)).ToList();
        var paginatedGetAuthorDtos = new PaginatedList<GetAuthorDto>(
            getAuthorDtos,
            getAuthorsByQueryParametersQuery.PageIndex,
            getAuthorsByQueryParametersQuery.TotalAmountOfItemsPerPage);
        return Result.Success<PaginatedList<GetAuthorDto>>(paginatedGetAuthorDtos);
    }
}
