namespace BookRentalManager.Application.Books.CommandHandlers;

public sealed class CreateBookForAuthorCommandHandler : ICommandHandler<CreateBookForAuthorCommand, BookForAuthorCreatedDto>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Book, BookForAuthorCreatedDto> _bookForAuthorCreatedDtoMapper;

    public CreateBookForAuthorCommandHandler(
        IRepository<Book> bookRepository,
        IRepository<Author> authorRepository,
        IMapper<Book, BookForAuthorCreatedDto> bookForAuthorCreatedDtoMapper)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _bookForAuthorCreatedDtoMapper = bookForAuthorCreatedDtoMapper;
    }

    public async Task<Result<BookForAuthorCreatedDto>> HandleAsync(
        CreateBookForAuthorCommand createBookForAuthorCommand,
        CancellationToken cancellationToken)
    {
        var authorByIdSpecification = new AuthorByIdSpecification(createBookForAuthorCommand.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdSpecification);
        if (author is null)
        {
            return Result.Fail<BookForAuthorCreatedDto>(
                nameof(HandleAsync),
                $"No author with the ID of '{createBookForAuthorCommand.AuthorId}' was found.");
        }
        Result<Edition> editionResult = Edition.Create(createBookForAuthorCommand.Edition);
        Result<Isbn> isbnResult = Isbn.Create(createBookForAuthorCommand.Isbn);
        Result combinedResults = Result.Combine(editionResult, isbnResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<BookForAuthorCreatedDto>(combinedResults.ErrorType, combinedResults.ErrorMessage);
        }
        var newBook = new Book(createBookForAuthorCommand.BookTitle, editionResult.Value!, isbnResult.Value!);
        Result addBookToAuthorResult = author.AddBook(newBook);
        if (!addBookToAuthorResult.IsSuccess)
        {
            return Result.Fail<BookForAuthorCreatedDto>(addBookToAuthorResult.ErrorType, addBookToAuthorResult.ErrorMessage);
        }
        await _bookRepository.CreateAsync(newBook, cancellationToken);
        return Result.Success<BookForAuthorCreatedDto>(_bookForAuthorCreatedDtoMapper.Map(newBook));
    }
}
