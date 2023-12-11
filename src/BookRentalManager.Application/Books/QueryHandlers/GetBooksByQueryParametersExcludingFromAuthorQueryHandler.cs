namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBooksByQueryParametersExcludingFromAuthorQueryHandler(
    IRepository<Author> authorRepository,
    IRepository<Book> bookRepository,
    ISortParametersMapper sortParametersMapper)
    : IRequestHandler<GetBooksByQueryParametersExcludingFromAuthorQuery, PaginatedList<GetBookDto>>
{
    private readonly IRepository<Author> _authorRepository = authorRepository;
    private readonly IRepository<Book> _bookRepository = bookRepository;
    private readonly ISortParametersMapper _sortParametersMapper = sortParametersMapper;

    public async Task<Result<PaginatedList<GetBookDto>>> HandleAsync(
        GetBooksByQueryParametersExcludingFromAuthorQuery getBooksByQueryParameterFromAuthor,
        CancellationToken cancellationToken)
    {
        var authorByIdWithBooksSpecification = new AuthorByIdWithBooksSpecification(getBooksByQueryParameterFromAuthor.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdWithBooksSpecification, cancellationToken);
        if (author is null)
        {
            return Result.Fail<PaginatedList<GetBookDto>>(
                RequestErrors.IdNotFoundError,
                $"No author with the ID of '{getBooksByQueryParameterFromAuthor.AuthorId}' was found.");
        }
        Result<string> convertedSortParametersResult = _sortParametersMapper.MapBookSortParameters(
            getBooksByQueryParameterFromAuthor.SortParameters);
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
        List<GetBookDto> getBookDtos = books
            .Select(book => new GetBookDto(book))
            .ToList();
        var paginatedGetBookDtos = new PaginatedList<GetBookDto>(
            getBookDtos,
            books.TotalAmountOfItems,
            books.TotalAmountOfPages,
            books.PageIndex,
            books.PageSize);
        return Result.Success(paginatedGetBookDtos);
    }
}
