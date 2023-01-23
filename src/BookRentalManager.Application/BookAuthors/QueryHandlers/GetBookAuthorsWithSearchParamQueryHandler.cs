namespace BookRentalManager.Application.BooksAuthors.QueryHandlers;

internal sealed class GetBookAuthorsWithSearchParamQueryHandler
    : IQueryHandler<GetBookAuthorsWithSearchParamQuery, IReadOnlyList<GetBookAuthorDto>>
{
    private readonly IRepository<BookAuthor> _bookAuthorRepository;
    private readonly IMapper<BookAuthor, GetBookAuthorDto> _getBookAuthorDtoMapper;

    public GetBookAuthorsWithSearchParamQueryHandler(
        IRepository<BookAuthor> bookAuthorRepository,
        IMapper<BookAuthor, GetBookAuthorDto> getBookAuthorDtoMapper)
    {
        _bookAuthorRepository = bookAuthorRepository;
        _getBookAuthorDtoMapper = getBookAuthorDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetBookAuthorDto>>> HandleAsync(
        GetBookAuthorsWithSearchParamQuery getBookAuthorsWithSearchParamQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<BookAuthor> bookAuthors = await _bookAuthorRepository.GetAllBySpecificationAsync(
            getBookAuthorsWithSearchParamQuery.PageIndex,
            getBookAuthorsWithSearchParamQuery.TotalItemsPerPage,
            new BookAuthorsWithSearchParamSpecification(getBookAuthorsWithSearchParamQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetBookAuthorDto> getBookAuthorDtos = (from bookAuthor in bookAuthors
                                                             select _getBookAuthorDtoMapper.Map(bookAuthor)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetBookAuthorDto>>(getBookAuthorDtos);
    }
}
