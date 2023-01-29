namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBookByIdQueryHandler : IQueryHandler<GetBookByIdQuery, GetBookDto>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _getBookDtoMapper;

    public GetBookByIdQueryHandler(
        IRepository<Author> authorRepository,
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> getBookDtoMapper)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _getBookDtoMapper = getBookDtoMapper;
    }

    public async Task<Result<GetBookDto>> HandleAsync(GetBookByIdQuery getBookByIdQuery, CancellationToken cancellationToken)
    {
        var authorByIdSpecification = new AuthorByIdSpecification(getBookByIdQuery.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdSpecification);
        if (author is null)
        {
            return Result.Fail<GetBookDto>($"No author with the ID of '{getBookByIdQuery.AuthorId}' was found.");
        }
        Guid bookId = (from book in author.Books
                       where book.Id == getBookByIdQuery.Id
                       select book.Id).FirstOrDefault();
        if (bookId.Equals(Guid.Empty))
        {
            return Result.Fail<GetBookDto>($"No book with the ID of '{getBookByIdQuery.Id}' was found for this author.");
        }
        Book? existingBook = await _bookRepository.GetFirstOrDefaultBySpecificationAsync(
            new BookByIdSpecification(bookId),
            cancellationToken);
        return Result.Success<GetBookDto>(_getBookDtoMapper.Map(existingBook!));
    }
}
