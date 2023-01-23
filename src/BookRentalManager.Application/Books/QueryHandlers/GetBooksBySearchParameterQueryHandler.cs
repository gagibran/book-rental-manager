namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBooksBySearchParameterQueryHandler
    : IQueryHandler<GetBooksBySearchParameterQuery, IReadOnlyList<GetBookDto>>
{
    private readonly IRepository<BookAuthor> _bookAuthorRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _getBookDtoMapper;

    public GetBooksBySearchParameterQueryHandler(
        IRepository<BookAuthor> bookAuthorRepository,
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> getBookDtoMapper)
    {
        _bookAuthorRepository = bookAuthorRepository;
        _bookRepository = bookRepository;
        _getBookDtoMapper = getBookDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetBookDto>>> HandleAsync(
        GetBooksBySearchParameterQuery getBooksBySearchParameterQuery,
        CancellationToken cancellationToken)
    {
        var bookAuthorByIdSpecification = new BookAuthorByIdSpecification(getBooksBySearchParameterQuery.BookAuthorId);
        BookAuthor? bookAuthor = await _bookAuthorRepository.GetFirstOrDefaultBySpecificationAsync(bookAuthorByIdSpecification);
        if (bookAuthor is null)
        {
            return Result.Fail<IReadOnlyList<GetBookDto>>($"No book author with the ID of '{getBooksBySearchParameterQuery.BookAuthorId}' was found.");
        }
        var booksByBookAuthorBooksSpecification = new BooksByBookAuthorBooksAndSearchParameterSpecification(
            bookAuthor.Books,
            getBooksBySearchParameterQuery.SearchParameter);
        IReadOnlyList<Book> books = await _bookRepository.GetAllBySpecificationAsync(
            getBooksBySearchParameterQuery.PageIndex,
            getBooksBySearchParameterQuery.TotalItemsPerPage,
            booksByBookAuthorBooksSpecification,
            cancellationToken);
        IReadOnlyList<GetBookDto> getBookDtos = (from book in books
                                                 select _getBookDtoMapper.Map(book)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetBookDto>>(getBookDtos);
    }
}
