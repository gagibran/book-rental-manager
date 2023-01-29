namespace BookRentalManager.Application.Books.CommandHandlers;

public sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, BookCreatedDto>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Book, BookCreatedDto> _bookCreatedDtoMapper;

    public CreateBookCommandHandler(
        IRepository<Book> bookRepository,
        IRepository<Author> authorRepository,
        IMapper<Book, BookCreatedDto> bookCreatedDtoMapper)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _bookCreatedDtoMapper = bookCreatedDtoMapper;
    }

    public async Task<Result<BookCreatedDto>> HandleAsync(CreateBookCommand createBookCommand, CancellationToken cancellationToken)
    {
        var authorByIdSpecification = new AuthorByIdSpecification(createBookCommand.AuthorId);
        Author? author = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdSpecification);
        if (author is null)
        {
            return Result.Fail<BookCreatedDto>(
                $"No author with the ID of '{createBookCommand.AuthorId}' was found.");
        }
        Result<Edition> editionResult = Edition.Create(createBookCommand.Edition);
        Result<Isbn> isbnResult = Isbn.Create(createBookCommand.Isbn);
        Result combinedResults = Result.Combine(editionResult, isbnResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<BookCreatedDto>(combinedResults.ErrorMessage);
        }
        var newBook = new Book(createBookCommand.BookTitle, editionResult.Value!, isbnResult.Value!);
        Result addBookToAuthorResult = author.AddBook(newBook);
        if (!addBookToAuthorResult.IsSuccess)
        {
            return Result.Fail<BookCreatedDto>(addBookToAuthorResult.ErrorMessage);
        }
        await _bookRepository.CreateAsync(newBook, cancellationToken);
        return Result.Success<BookCreatedDto>(_bookCreatedDtoMapper.Map(newBook));
    }
}
