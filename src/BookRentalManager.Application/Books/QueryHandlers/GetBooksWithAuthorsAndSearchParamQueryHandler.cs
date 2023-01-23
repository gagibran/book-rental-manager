using BookRentalManager.Application.Books.Queries;

namespace BookRentalManager.Application.Books.QueryHandlers;

public sealed class GetBooksWithSearchParamQueryHandler
    : IQueryHandler<GetBooksWithSearchParamQuery, IReadOnlyList<GetBookDto>>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _getBookDtoMapper;

    public GetBooksWithSearchParamQueryHandler(
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> getBookDtoMapper)
    {
        _bookRepository = bookRepository;
        _getBookDtoMapper = getBookDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetBookDto>>> HandleAsync(
        GetBooksWithSearchParamQuery getBooksWithSearchParamQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Book> books = await _bookRepository.GetAllBySpecificationAsync(
            getBooksWithSearchParamQuery.PageIndex,
            getBooksWithSearchParamQuery.TotalItemsPerPage,
            new BooksWithSearchParamSpecification(getBooksWithSearchParamQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetBookDto> getBookDtos = (from book in books
                                                 select _getBookDtoMapper.Map(book)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetBookDto>>(getBookDtos);
    }
}
