namespace BookRentalManager.Application.Books.CommandHandlers;

public sealed class CreateBookForAuthorCommandHandler : ICommandHandler<CreateBookForAuthorCommand, BookCreatedDto>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Book, BookCreatedDto> _bookCreatedDtoMapper;

    public CreateBookForAuthorCommandHandler(
        IRepository<Book> bookRepository,
        IRepository<Author> authorRepository,
        IMapper<Book, BookCreatedDto> bookCreatedDtoMapper)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _bookCreatedDtoMapper = bookCreatedDtoMapper;
    }

    public async Task<Result<BookCreatedDto>> HandleAsync(CreateBookForAuthorCommand createBookForAuthorCommand, CancellationToken cancellationToken)
    {
        var authorByIdSpecification = new AuthorByIdSpecification(createBookForAuthorCommand.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdSpecification);
        if (author is null)
        {
            return Result.Fail<BookCreatedDto>(
                $"No author with the ID of '{createBookForAuthorCommand.AuthorId}' was found.");
        }
        Result<Edition> editionResult = Edition.Create(createBookForAuthorCommand.Edition);
        Result<Isbn> isbnResult = Isbn.Create(createBookForAuthorCommand.Isbn);
        Result combinedResults = Result.Combine(editionResult, isbnResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<BookCreatedDto>(combinedResults.ErrorMessage);
        }
        var newBook = new Book(createBookForAuthorCommand.BookTitle, editionResult.Value!, isbnResult.Value!);
        Result addBookToAuthorResult = author.AddBook(newBook);
        if (!addBookToAuthorResult.IsSuccess)
        {
            return Result.Fail<BookCreatedDto>(addBookToAuthorResult.ErrorMessage);
        }
        await _bookRepository.CreateAsync(newBook, cancellationToken);
        return Result.Success<BookCreatedDto>(_bookCreatedDtoMapper.Map(newBook));
    }
}
