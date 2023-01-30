namespace BookRentalManager.Application.Books.CommandHandlers;

public sealed class CreateBookForAuthorCommandHandler : ICommandHandler<CreateBookForAuthorCommand, BookCreatedForAuthorDto>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Book, BookCreatedForAuthorDto> _bookCreatedForAuthorDtoMapper;

    public CreateBookForAuthorCommandHandler(
        IRepository<Book> bookRepository,
        IRepository<Author> authorRepository,
        IMapper<Book, BookCreatedForAuthorDto> bookCreatedForAuthorDtoMapper)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _bookCreatedForAuthorDtoMapper = bookCreatedForAuthorDtoMapper;
    }

    public async Task<Result<BookCreatedForAuthorDto>> HandleAsync(
        CreateBookForAuthorCommand createBookForAuthorCommand,
        CancellationToken cancellationToken)
    {
        var authorByIdSpecification = new AuthorByIdSpecification(createBookForAuthorCommand.AuthorId);
        Author? existingAuthor = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdSpecification);
        Result<Author?> existingAuthorResult = Result.Success<Author?>(existingAuthor);
        Result<Edition> editionResult = Edition.Create(createBookForAuthorCommand.Edition);
        Result<Isbn> isbnResult = Isbn.Create(createBookForAuthorCommand.Isbn);
        if (existingAuthor is null)
        {
            existingAuthorResult = Result.Fail<Author?>(
                "authorId",
                $"No author with the ID of '{createBookForAuthorCommand.AuthorId}' was found.");
        }
        Result combinedResults = Result.Combine(existingAuthorResult, editionResult, isbnResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<BookCreatedForAuthorDto>(combinedResults.ErrorType, combinedResults.ErrorMessage);
        }
        var newBook = new Book(createBookForAuthorCommand.BookTitle, editionResult.Value!, isbnResult.Value!);
        Result addBookToAuthorResult = existingAuthor!.AddBook(newBook);
        if (!addBookToAuthorResult.IsSuccess)
        {
            return Result.Fail<BookCreatedForAuthorDto>(addBookToAuthorResult.ErrorType, addBookToAuthorResult.ErrorMessage);
        }
        await _bookRepository.CreateAsync(newBook, cancellationToken);
        return Result.Success<BookCreatedForAuthorDto>(_bookCreatedForAuthorDtoMapper.Map(newBook));
    }
}
