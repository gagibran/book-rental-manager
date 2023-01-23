namespace BookRentalManager.Application.Books.CommandHandlers;

public sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<BookAuthor> _bookAuthorRepository;

    public CreateBookCommandHandler(IRepository<Book> bookRepository, IRepository<BookAuthor> bookAuthorRepository)
    {
        _bookRepository = bookRepository;
        _bookAuthorRepository = bookAuthorRepository;
    }

    public async Task<Result> HandleAsync(CreateBookCommand createBookCommand, CancellationToken cancellationToken)
    {
        var bookAuthorByIdSpecification = new BookAuthorByIdSpecification(createBookCommand.BookAuthorId);
        BookAuthor? bookAuthor = await _bookAuthorRepository.GetFirstOrDefaultBySpecificationAsync(bookAuthorByIdSpecification);
        if (bookAuthor is null)
        {
            return Result.Fail($"No book author with the ID of '{createBookCommand.BookAuthorId}' was found.");
        }
        var bookInBookAuthorBooksByTitleSpecification = new BookInBookAuthorBooksByTitleSpecification(
            bookAuthor.Books,
            createBookCommand.Book.BookTitle);
        Book? book = await _bookRepository.GetFirstOrDefaultBySpecificationAsync(bookInBookAuthorBooksByTitleSpecification);
        if (book is not null)
        {
            return Result.Fail($"A book with the title '{createBookCommand.Book.BookTitle}' already exists for this book author.");
        }
        bookAuthor.AddBook(createBookCommand.Book);
        await _bookRepository.CreateAsync(createBookCommand.Book, cancellationToken);
        return Result.Success();
    }
}
