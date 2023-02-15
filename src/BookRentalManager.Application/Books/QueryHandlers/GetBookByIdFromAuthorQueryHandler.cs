namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBookByIdFromAuthorQueryHandler : IQueryHandler<GetBookByIdFromAuthorQuery, GetBookDto>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, GetBookDto> _bookToGetBookDtoMapper;

    public GetBookByIdFromAuthorQueryHandler(
        IRepository<Author> authorRepository,
        IRepository<Book> bookRepository,
        IMapper<Book, GetBookDto> bookToGetBookDtoMapper)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _bookToGetBookDtoMapper = bookToGetBookDtoMapper;
    }

    public async Task<Result<GetBookDto>> HandleAsync(GetBookByIdFromAuthorQuery getBookByIdFromAuthorQuery, CancellationToken cancellationToken)
    {
        var authorByIdSpecification = new AuthorByIdSpecification(getBookByIdFromAuthorQuery.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdSpecification);
        if (author is null)
        {
            return Result.Fail<GetBookDto>("authorId", $"No author with the ID of '{getBookByIdFromAuthorQuery.AuthorId}' was found.");
        }
        Guid bookId = (from book in author.Books
                       where book.Id == getBookByIdFromAuthorQuery.Id
                       select book.Id).FirstOrDefault();
        if (bookId.Equals(Guid.Empty))
        {
            return Result.Fail<GetBookDto>("bookId", $"No book with the ID of '{getBookByIdFromAuthorQuery.Id}' was found for this author.");
        }
        var bookByIdSpecification = new BookByIdSpecification(bookId);
        Book? existingBook = await _bookRepository.GetFirstOrDefaultBySpecificationAsync(bookByIdSpecification, cancellationToken);
        return Result.Success<GetBookDto>(_bookToGetBookDtoMapper.Map(existingBook!));
    }
}
