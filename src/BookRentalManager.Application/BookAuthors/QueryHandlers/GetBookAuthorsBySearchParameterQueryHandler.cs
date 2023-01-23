namespace BookRentalManager.Application.BooksAuthors.QueryHandlers;

internal sealed class GetBookAuthorsBySearchParameterQueryHandler
    : IQueryHandler<GetBookAuthorsBySearchParameterQuery, IReadOnlyList<GetBookAuthorDto>>
{
    private readonly IRepository<BookAuthor> _bookAuthorRepository;
    private readonly IMapper<BookAuthor, GetBookAuthorDto> _getBookAuthorDtoMapper;

    public GetBookAuthorsBySearchParameterQueryHandler(
        IRepository<BookAuthor> bookAuthorRepository,
        IMapper<BookAuthor, GetBookAuthorDto> getBookAuthorDtoMapper)
    {
        _bookAuthorRepository = bookAuthorRepository;
        _getBookAuthorDtoMapper = getBookAuthorDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetBookAuthorDto>>> HandleAsync(
        GetBookAuthorsBySearchParameterQuery getBookAuthorsBySearchParameterQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<BookAuthor> bookAuthors = await _bookAuthorRepository.GetAllBySpecificationAsync(
            getBookAuthorsBySearchParameterQuery.PageIndex,
            getBookAuthorsBySearchParameterQuery.TotalItemsPerPage,
            new BookAuthorsBySearchParameterSpecification(getBookAuthorsBySearchParameterQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetBookAuthorDto> getBookAuthorDtos = (from bookAuthor in bookAuthors
                                                             select _getBookAuthorDtoMapper.Map(bookAuthor)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetBookAuthorDto>>(getBookAuthorDtos);
    }
}
