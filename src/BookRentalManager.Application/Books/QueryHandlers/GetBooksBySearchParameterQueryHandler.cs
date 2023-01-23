namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBooksBySearchParameterQueryHandler
    : IQueryHandler<GetBooksBySearchParameterQuery, IReadOnlyList<GetBookDto>>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _getBookDtoMapper;

    public GetBooksBySearchParameterQueryHandler(
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> getBookDtoMapper)
    {
        _bookRepository = bookRepository;
        _getBookDtoMapper = getBookDtoMapper;
    }

    public async Task<Result<IReadOnlyList<GetBookDto>>> HandleAsync(
        GetBooksBySearchParameterQuery getBooksBySearchParameterQuery,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Book> books = await _bookRepository.GetAllBySpecificationAsync(
            getBooksBySearchParameterQuery.PageIndex,
            getBooksBySearchParameterQuery.TotalItemsPerPage,
            new BooksBySearchParameterSpecification(getBooksBySearchParameterQuery.SearchParameter),
            cancellationToken);
        IReadOnlyList<GetBookDto> getBookDtos = (from book in books
                                                 select _getBookDtoMapper.Map(book)).ToList().AsReadOnly();
        return Result.Success<IReadOnlyList<GetBookDto>>(getBookDtos);
    }
}
