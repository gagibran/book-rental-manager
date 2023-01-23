namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBookByIdQueryHandler
    : IQueryHandler<GetBookByIdQuery, GetBookDto>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _getBookDtoMapper;

    public GetBookByIdQueryHandler(
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> getBookDtoMapper)
    {
        _bookRepository = bookRepository;
        _getBookDtoMapper = getBookDtoMapper;
    }

    public async Task<Result<GetBookDto>> HandleAsync(GetBookByIdQuery getBookByIdQuery, CancellationToken cancellationToken)
    {
        Book? book = await _bookRepository.GetFirstOrDefaultBySpecificationAsync(
            new BookByIdSpecification(getBookByIdQuery.Id),
            cancellationToken);
        if (book is null)
        {
            return Result.Fail<GetBookDto>($"No book with the ID of '{getBookByIdQuery.Id} was found.");
        }
        return Result.Success<GetBookDto>(_getBookDtoMapper.Map(book));
    }
}
