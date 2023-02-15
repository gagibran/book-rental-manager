namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorsByQueryParametersQueryHandler
    : IQueryHandler<GetAuthorsByQueryParametersQuery, PaginatedList<GetAuthorDto>>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Author, GetAuthorDto> _authorToGetAuthorDtoMapper;

    public GetAuthorsByQueryParametersQueryHandler(
        IRepository<Author> authorRepository,
        IMapper<Author, GetAuthorDto> authorToGetAuthorDtoMapper)
    {
        _authorRepository = authorRepository;
        _authorToGetAuthorDtoMapper = authorToGetAuthorDtoMapper;
    }

    public async Task<Result<PaginatedList<GetAuthorDto>>> HandleAsync(
        GetAuthorsByQueryParametersQuery getAuthorsByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        PaginatedList<Author> authors = await _authorRepository.GetAllBySpecificationAsync(
            getAuthorsByQueryParametersQuery.PageIndex,
            getAuthorsByQueryParametersQuery.PageSize,
            new AuthorsBySearchParameterSpecification(getAuthorsByQueryParametersQuery.SearchParameter),
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
