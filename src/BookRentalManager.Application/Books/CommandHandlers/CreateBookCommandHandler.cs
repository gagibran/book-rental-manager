namespace BookRentalManager.Application.Books.CommandHandlers;

public sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, BookCreatedDto>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<BookAuthor> _bookAuthorRepository;
    private readonly IMapper<Book, BookCreatedDto> _bookCreatedDtoMapper;

    public CreateBookCommandHandler(
        IRepository<Book> bookRepository,
        IRepository<BookAuthor> bookAuthorRepository,
        IMapper<Book, BookCreatedDto> bookCreatedDtoMapper)
    {
        _bookRepository = bookRepository;
        _bookAuthorRepository = bookAuthorRepository;
        _bookCreatedDtoMapper = bookCreatedDtoMapper;
    }

    public async Task<Result<BookCreatedDto>> HandleAsync(CreateBookCommand createBookCommand, CancellationToken cancellationToken)
    {
        var bookAuthorByIdSpecification = new BookAuthorByIdSpecification(createBookCommand.BookAuthorId);
        BookAuthor? bookAuthor = await _bookAuthorRepository.GetFirstOrDefaultBySpecificationAsync(bookAuthorByIdSpecification);
        if (bookAuthor is null)
        {
            return Result.Fail<BookCreatedDto>(
                $"No book author with the ID of '{createBookCommand.BookAuthorId}' was found.");
        }
        Result<Edition> editionResult = Edition.Create(createBookCommand.CreateBookDto.Edition);
        Result<Isbn> isbnResult = Isbn.Create(createBookCommand.CreateBookDto.Isbn);
        Result combinedResults = Result.Combine(editionResult, isbnResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<BookCreatedDto>(combinedResults.ErrorMessage);
        }
        var newBook = new Book(createBookCommand.CreateBookDto.BookTitle, editionResult.Value!, isbnResult.Value!);
        Result addBookToBookAuthorResult = bookAuthor.AddBook(newBook);
        if (!addBookToBookAuthorResult.IsSuccess)
        {
            return Result.Fail<BookCreatedDto>(addBookToBookAuthorResult.ErrorMessage);
        }
        await _bookRepository.CreateAsync(newBook, cancellationToken);
        return Result.Success<BookCreatedDto>(_bookCreatedDtoMapper.Map(newBook));
    }
}
