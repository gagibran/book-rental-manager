namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBooksByQueryParameterFromAuthorQueryHandler
    : IQueryHandler<GetBooksByQueryParameterFromAuthorQuery, IReadOnlyList<GetBookDto>>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _getBookDtoMapper;

    public GetBooksByQueryParameterFromAuthorQueryHandler(
        IRepository<Author> authorRepository,
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> getBookDtoMapper)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _getBookDtoMapper = getBookDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetBookDto>>> HandleAsync(
        GetBooksByQueryParameterFromAuthorQuery getBooksByQueryParameterFromAuthor,
        CancellationToken cancellationToken)
    {
        var authorByIdSpecification = new AuthorByIdSpecification(getBooksByQueryParameterFromAuthor.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdSpecification);
        if (author is null)
        {
            return Result.Fail<IReadOnlyList<GetBookDto>>($"No author with the ID of '{getBooksByQueryParameterFromAuthor.AuthorId}' was found.");
        }
        var booksInAuthorBooksAndQueryParameterSpecification = new BooksFromAuthorBooksByQueryParameterSpecification(
            author.Books,
            getBooksByQueryParameterFromAuthor.SearchParameter);
        IReadOnlyList<Book> books = await _bookRepository.GetAllBySpecificationAsync(
            getBooksByQueryParameterFromAuthor.PageIndex,
            getBooksByQueryParameterFromAuthor.TotalItemsPerPage,
            booksInAuthorBooksAndQueryParameterSpecification,
            cancellationToken);
        IReadOnlyList<GetBookDto> getBookDtos = (from book in books
                                                 select _getBookDtoMapper.Map(book)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetBookDto>>(getBookDtos);
    }
}
