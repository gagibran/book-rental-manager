namespace BookRentalManager.Application.Authors.QueryHandlers;

internal sealed class GetAuthorsByQueryParametersQueryHandler(
    IRepository<Author> authorRepository,
    ISortParametersMapper sortParametersMapper)
    : IRequestHandler<GetAuthorsByQueryParametersQuery, PaginatedList<GetAuthorDto>>
{
    private readonly IRepository<Author> _authorRepository = authorRepository;
    private readonly ISortParametersMapper _sortParametersMapper = sortParametersMapper;

    public async Task<Result<PaginatedList<GetAuthorDto>>> HandleAsync(
        GetAuthorsByQueryParametersQuery getAuthorsByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        Result<string> convertedSortParametersResult = _sortParametersMapper.MapAuthorSortParameters(
            getAuthorsByQueryParametersQuery.SortParameters);
        if (!convertedSortParametersResult.IsSuccess)
        {
            return Result.Fail<PaginatedList<GetAuthorDto>>(
                convertedSortParametersResult.ErrorType,
                convertedSortParametersResult.ErrorMessage);
        }
        var authorsBySearchParameterWithBooksSpecification = new AuthorsBySearchParameterWithBooksSpecification(
            getAuthorsByQueryParametersQuery.SearchParameter,
            convertedSortParametersResult.Value!);
        PaginatedList<Author> authors = await _authorRepository.GetAllBySpecificationAsync(
            getAuthorsByQueryParametersQuery.PageIndex,
            getAuthorsByQueryParametersQuery.PageSize,
            authorsBySearchParameterWithBooksSpecification,
            cancellationToken);
        List<GetAuthorDto> getAuthorDtos = authors
            .Select(author => new GetAuthorDto(author))
            .ToList();
        var paginatedGetAuthorDtos = new PaginatedList<GetAuthorDto>(
            getAuthorDtos,
            authors.TotalAmountOfItems,
            authors.TotalAmountOfPages,
            authors.PageIndex,
            authors.PageSize);
        return Result.Success(paginatedGetAuthorDtos);
    }
}
