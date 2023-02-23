namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorsByQueryParametersQueryHandler
    : IQueryHandler<GetAuthorsByQueryParametersQuery, PaginatedList<GetAuthorDto>>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Author, GetAuthorDto> _authorToGetAuthorDtoMapper;
    private readonly IMapper<AuthorSortParameters, Result<string>> _authorSortParametersMapper;

    public GetAuthorsByQueryParametersQueryHandler(
        IRepository<Author> authorRepository,
        IMapper<Author, GetAuthorDto> authorToGetAuthorDtoMapper,
        IMapper<AuthorSortParameters, Result<string>> authorSortParametersMapper)
    {
        _authorRepository = authorRepository;
        _authorToGetAuthorDtoMapper = authorToGetAuthorDtoMapper;
        _authorSortParametersMapper = authorSortParametersMapper;
    }

    public async Task<Result<PaginatedList<GetAuthorDto>>> HandleAsync(
        GetAuthorsByQueryParametersQuery getAuthorsByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        Result<string> convertedSorParametersResult = _authorSortParametersMapper.Map(
            new AuthorSortParameters(getAuthorsByQueryParametersQuery.SortParameters));
        if (!convertedSorParametersResult.IsSuccess)
        {
            return Result.Fail<PaginatedList<GetAuthorDto>>(
                convertedSorParametersResult.ErrorType,
                convertedSorParametersResult.ErrorMessage);
        }
        var authorsBySearchParameterWithBooksSpecification = new AuthorsBySearchParameterWithBooksSpecification(
            getAuthorsByQueryParametersQuery.SearchParameter,
            convertedSorParametersResult.Value!);
        PaginatedList<Author> authors = await _authorRepository.GetAllBySpecificationAsync(
            getAuthorsByQueryParametersQuery.PageIndex,
            getAuthorsByQueryParametersQuery.PageSize,
            authorsBySearchParameterWithBooksSpecification,
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
