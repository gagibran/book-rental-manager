namespace BookRentalManager.Application.BookAuthorCqrs.QueryHandlers;

internal sealed class GetBookAuthorsQueryHandler
    : IQueryHandler<GetBookAuthorsQuery, IReadOnlyList<GetBookAuthorDto>>
{
    private readonly IRepository<BookAuthor> _bookAuthorRepository;
    private readonly IMapper<BookAuthor, GetBookAuthorDto> _getBookAuthorDtoMapper;

    public GetBookAuthorsQueryHandler(
        IRepository<BookAuthor> bookAuthorRepository,
        IMapper<BookAuthor, GetBookAuthorDto> getBookAuthorDtoMapper
    )
    {
        _bookAuthorRepository = bookAuthorRepository;
        _getBookAuthorDtoMapper = getBookAuthorDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetBookAuthorDto>>> HandleAsync(
        GetBookAuthorsQuery getBookAuthorsQuery,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<BookAuthor> bookAuthors = await _bookAuthorRepository.GetAllAsync(
            getBookAuthorsQuery.PageIndex,
            getBookAuthorsQuery.TotalItemsPerPage,
            new BookAuthorsWithBooksSpecification(),
            cancellationToken
        );
        IEnumerable<GetBookAuthorDto> getBookAuthorDtos = from bookAuthor in bookAuthors
                                                          select _getBookAuthorDtoMapper.Map(bookAuthor);
        return Result.Success<IReadOnlyList<GetBookAuthorDto>>(getBookAuthorDtos.ToList());
    }
}
