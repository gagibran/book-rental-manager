namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBookByIdQueryHandler : IQueryHandler<GetBookByIdQuery, GetBookDto>
{
    private readonly IRepository<BookAuthor> _bookAuthorRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _getBookDtoMapper;

    public GetBookByIdQueryHandler(
        IRepository<BookAuthor> bookAuthorRepository,
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> getBookDtoMapper)
    {
        _bookAuthorRepository = bookAuthorRepository;
        _bookRepository = bookRepository;
        _getBookDtoMapper = getBookDtoMapper;
    }

    public async Task<Result<GetBookDto>> HandleAsync(GetBookByIdQuery getBookByIdQuery, CancellationToken cancellationToken)
    {
        var bookAuthorByIdSpecification = new BookAuthorByIdSpecification(getBookByIdQuery.BookAuthorId);
        BookAuthor? bookAuthor = await _bookAuthorRepository.GetFirstOrDefaultBySpecificationAsync(bookAuthorByIdSpecification);
        if (bookAuthor is null)
        {
            return Result.Fail<GetBookDto>($"No book author with the ID of '{getBookByIdQuery.BookAuthorId} was found.");
        }
        Guid bookId = (from book in bookAuthor!.Books
                       where book.Id == getBookByIdQuery.Id
                       select book.Id).FirstOrDefault();
        if (bookId.Equals(Guid.Empty))
        {
            return Result.Fail<GetBookDto>($"No book with the ID of '{getBookByIdQuery.Id} was found for this book author.");
        }
        Book? existingBook = await _bookRepository.GetFirstOrDefaultBySpecificationAsync(
            new BookByIdSpecification(bookId),
            cancellationToken);
        return Result.Success<GetBookDto>(_getBookDtoMapper.Map(existingBook!));
    }
}
