namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBooksBySearchParameterQueryHandler
    : IQueryHandler<GetBooksBySearchParameterQuery, IReadOnlyList<GetBookDto>>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _getBookDtoMapper;

    public GetBooksBySearchParameterQueryHandler(
        IRepository<Author> authorRepository,
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> getBookDtoMapper)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _getBookDtoMapper = getBookDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetBookDto>>> HandleAsync(
        GetBooksBySearchParameterQuery getBooksBySearchParameterQuery,
        CancellationToken cancellationToken)
    {
        var authorByIdSpecification = new AuthorByIdSpecification(getBooksBySearchParameterQuery.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdSpecification);
        if (author is null)
        {
            return Result.Fail<IReadOnlyList<GetBookDto>>($"No author with the ID of '{getBooksBySearchParameterQuery.AuthorId}' was found.");
        }
        var booksInAuthorBooksAndSearchParameterSpecification = new BooksFromAuthorBooksAndSearchParameterSpecification(
            author.Books,
            getBooksBySearchParameterQuery.SearchParameter);
        IReadOnlyList<Book> books = await _bookRepository.GetAllBySpecificationAsync(
            getBooksBySearchParameterQuery.PageIndex,
            getBooksBySearchParameterQuery.TotalItemsPerPage,
            booksInAuthorBooksAndSearchParameterSpecification,
            cancellationToken);
        IReadOnlyList<GetBookDto> getBookDtos = (from book in books
                                                 select _getBookDtoMapper.Map(book)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetBookDto>>(getBookDtos);
    }
}
