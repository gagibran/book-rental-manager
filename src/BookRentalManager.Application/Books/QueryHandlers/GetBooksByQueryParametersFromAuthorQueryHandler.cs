namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBooksByQueryParametersFromAuthorQueryHandler
    : IQueryHandler<GetBooksByQueryParametersFromAuthorQuery, PaginatedList<GetBookDto>>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _bookToGetBookDtoMapper;

    public GetBooksByQueryParametersFromAuthorQueryHandler(
        IRepository<Author> authorRepository,
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> bookToGetBookDtoMapper)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _bookToGetBookDtoMapper = bookToGetBookDtoMapper;
    }

    public async Task<Result<PaginatedList<GetBookDto>>> HandleAsync(
        GetBooksByQueryParametersFromAuthorQuery getBooksByQueryParameterFromAuthor,
        CancellationToken cancellationToken)
    {
        var authorByIdSpecification = new AuthorByIdSpecification(getBooksByQueryParameterFromAuthor.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdSpecification);
        if (author is null)
        {
            return Result.Fail<PaginatedList<GetBookDto>>(
                "authorId",
                $"No author with the ID of '{getBooksByQueryParameterFromAuthor.AuthorId}' was found.");
        }
        var booksInAuthorBooksAndQueryParameterSpecification = new BooksBySearchParameterInBooksFromAuthorSpecification(
            author.Books,
            getBooksByQueryParameterFromAuthor.SearchParameter);
        PaginatedList<Book> books = await _bookRepository.GetAllBySpecificationAsync(
            getBooksByQueryParameterFromAuthor.PageIndex,
            getBooksByQueryParameterFromAuthor.PageSize,
            booksInAuthorBooksAndQueryParameterSpecification,
            cancellationToken);
        List<GetBookDto> getBookDtos = (from book in books
                                        select _bookToGetBookDtoMapper.Map(book)).ToList();
        var paginatedGetBookDtos = new PaginatedList<GetBookDto>(
            getBookDtos,
            books.TotalAmountOfItems,
            books.TotalAmountOfPages,
            books.PageIndex,
            books.PageSize);
        return Result.Success<PaginatedList<GetBookDto>>(paginatedGetBookDtos);
    }
}
