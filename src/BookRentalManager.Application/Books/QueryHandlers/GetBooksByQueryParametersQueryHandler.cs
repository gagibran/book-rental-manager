namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBooksByQueryParametersQueryHandler
    : IRequestHandler<GetBooksByQueryParametersQuery, PaginatedList<GetBookDto>>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _bookToGetBookDtoMapper;
    private readonly IMapper<BookSortParameters, Result<string>> _bookSortParametersMapper;

    public GetBooksByQueryParametersQueryHandler(
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> bookToGetBookDtoMapper,
        IMapper<BookSortParameters, Result<string>> bookSortParametersMapper)
    {
        _bookRepository = bookRepository;
        _bookToGetBookDtoMapper = bookToGetBookDtoMapper;
        _bookSortParametersMapper = bookSortParametersMapper;
    }

    public async Task<Result<PaginatedList<GetBookDto>>> HandleAsync(
        GetBooksByQueryParametersQuery getBooksByQueryParametersQuery,
        CancellationToken cancellationToken)
    {
        Result<string> convertedSortParametersResult = _bookSortParametersMapper.Map(
            new BookSortParameters(getBooksByQueryParametersQuery.SortParameters));
        if (!convertedSortParametersResult.IsSuccess)
        {
            return Result.Fail<PaginatedList<GetBookDto>>(
                convertedSortParametersResult.ErrorType,
                convertedSortParametersResult.ErrorMessage);
        }
        var booksBySearchParameterWithAuthorsAndCustomersSpecification = new BooksBySearchParameterWithAuthorsAndCustomersSpecification(
            getBooksByQueryParametersQuery.SearchParameter,
            convertedSortParametersResult.Value!);
        PaginatedList<Book> books = await _bookRepository.GetAllBySpecificationAsync(
            getBooksByQueryParametersQuery.PageIndex,
            getBooksByQueryParametersQuery.PageSize,
            booksBySearchParameterWithAuthorsAndCustomersSpecification,
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
