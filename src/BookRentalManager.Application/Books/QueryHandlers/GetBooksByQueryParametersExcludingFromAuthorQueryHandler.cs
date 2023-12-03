namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBooksByQueryParametersExcludingFromAuthorQueryHandler
    : IRequestHandler<GetBooksByQueryParametersExcludingFromAuthorQuery, PaginatedList<GetBookDto>>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _bookToGetBookDtoMapper;
    private readonly IMapper<BookSortParameters, Result<string>> _bookSortParametersMapper;

    public GetBooksByQueryParametersExcludingFromAuthorQueryHandler(
        IRepository<Author> authorRepository,
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> bookToGetBookDtoMapper,
        IMapper<BookSortParameters, Result<string>> bookSortParametersMapper)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _bookToGetBookDtoMapper = bookToGetBookDtoMapper;
        _bookSortParametersMapper = bookSortParametersMapper;
    }

    public async Task<Result<PaginatedList<GetBookDto>>> HandleAsync(
        GetBooksByQueryParametersExcludingFromAuthorQuery getBooksByQueryParameterFromAuthor,
        CancellationToken cancellationToken)
    {
        var authorByIdWithBooksSpecification = new AuthorByIdWithBooksSpecification(getBooksByQueryParameterFromAuthor.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdWithBooksSpecification, cancellationToken);
        if (author is null)
        {
            return Result.Fail<PaginatedList<GetBookDto>>(
                "authorId",
                $"No author with the ID of '{getBooksByQueryParameterFromAuthor.AuthorId}' was found.");
        }
        Result<string> convertedSortParametersResult = _bookSortParametersMapper.Map(
            new BookSortParameters(getBooksByQueryParameterFromAuthor.SortParameters));
        if (!convertedSortParametersResult.IsSuccess)
        {
            return Result.Fail<PaginatedList<GetBookDto>>(
                convertedSortParametersResult.ErrorType,
                convertedSortParametersResult.ErrorMessage);
        }
        var booksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecification = new BooksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecification(
            author.Books,
            getBooksByQueryParameterFromAuthor.SearchParameter,
            convertedSortParametersResult.Value!);
        PaginatedList<Book> books = await _bookRepository.GetAllBySpecificationAsync(
            getBooksByQueryParameterFromAuthor.PageIndex,
            getBooksByQueryParameterFromAuthor.PageSize,
            booksBySearchParameterWithAuthorsAndCustomersExcludingBooksFromAuthorSpecification,
            cancellationToken);
        List<GetBookDto> getBookDtos = (from book in books
                                        select _bookToGetBookDtoMapper.Map(book)).ToList();
        var paginatedGetBookDtos = new PaginatedList<GetBookDto>(
            getBookDtos,
            books.TotalAmountOfItems,
            books.TotalAmountOfPages,
            books.PageIndex,
            books.PageSize);
        return Result.Success(paginatedGetBookDtos);
    }
}
