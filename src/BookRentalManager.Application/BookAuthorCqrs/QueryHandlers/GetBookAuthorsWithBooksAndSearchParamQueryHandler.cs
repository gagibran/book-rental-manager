namespace BookRentalManager.Application.BookAuthorCqrs.QueryHandlers;

internal sealed class GetBookAuthorsWithBooksAndSearchParamQueryHandler
    : IQueryHandler<GetBookAuthorsWithBooksAndSearchParamQuery, IReadOnlyList<GetBookAuthorDto>>
{
    private readonly IRepository<BookAuthor> _bookAuthorRepository;
    private readonly IMapper<BookAuthor, GetBookAuthorDto> _getBookAuthorDtoMapper;

    public GetBookAuthorsWithBooksAndSearchParamQueryHandler(
        IRepository<BookAuthor> bookAuthorRepository,
        IMapper<BookAuthor, GetBookAuthorDto> getBookAuthorDtoMapper)
    {
        _bookAuthorRepository = bookAuthorRepository;
        _getBookAuthorDtoMapper = getBookAuthorDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetBookAuthorDto>>> HandleAsync(
        GetBookAuthorsWithBooksAndSearchParamQuery getBookAuthorsWithBooksAndSearchParamQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<BookAuthor> bookAuthors = await _bookAuthorRepository.GetAllBySpecificationAsync(
            getBookAuthorsWithBooksAndSearchParamQuery.PageIndex,
            getBookAuthorsWithBooksAndSearchParamQuery.TotalItemsPerPage,
            new BookAuthorsWithBooksAndSearchParamSpecification(getBookAuthorsWithBooksAndSearchParamQuery.SearchParameter),
            cancellationToken);
        IEnumerable<GetBookAuthorDto> getBookAuthorDtos = from bookAuthor in bookAuthors
                                                          select _getBookAuthorDtoMapper.Map(bookAuthor);
        return Result.Success<IReadOnlyList<GetBookAuthorDto>>(getBookAuthorDtos.ToList());
    }
}
