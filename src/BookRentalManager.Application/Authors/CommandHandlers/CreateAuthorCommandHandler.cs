namespace BookRentalManager.Application.Authors.CommandHandlers;

internal sealed class CreateAuthorCommandHandler : ICommandHandler<CreateAuthorCommand, AuthorCreatedDto>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Author, AuthorCreatedDto> _authorToAuthorCreatedDtoMapper;

    public CreateAuthorCommandHandler(
        IRepository<Author> authorRepository,
        IRepository<Book> bookRepository,
        IMapper<Author, AuthorCreatedDto> authorToAuthorCreatedDtoMapper)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _authorToAuthorCreatedDtoMapper = authorToAuthorCreatedDtoMapper;
    }

    public async Task<Result<AuthorCreatedDto>> HandleAsync(
        CreateAuthorCommand createAuthorCommand,
        CancellationToken cancellationToken)
    {
        FullName authorFullName = FullName.Create(createAuthorCommand.FirstName, createAuthorCommand.LastName).Value!;
        var authorByFullNameSpecification = new AuthorByFullNameSpecification(authorFullName.CompleteName);
        Author? existingAuthor = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByFullNameSpecification);
        Result existingAuthorResult = Result.Success();
        if (existingAuthor is not null)
        {
            existingAuthorResult = Result.Fail(
                "authorAlreadyExists",
                $"An author named '{existingAuthor.FullName.CompleteName}' already exists.");
        }
        Result<FullName> authorFullNameResult = FullName.Create(createAuthorCommand.FirstName, createAuthorCommand.LastName);
        Result bookEditionResult = Result.Success();
        Result bookIsbnResult = Result.Success();
        Result bookAlreadyExists = Result.Success();
        Result addBookToAuthor = Result.Success();
        var newAuthor = new Author(authorFullNameResult.Value!);
        var booksToCreate = new List<Book>();
        var isbns = new List<string>();
        foreach (CreateBookForAuthorDto book in createAuthorCommand.Books)
        {
            bookEditionResult = Edition.Create(book.Edition);
            bookIsbnResult = Isbn.Create(book.Isbn);
            if (!bookEditionResult.IsSuccess || !bookIsbnResult.IsSuccess)
            {
                break;
            }
            Isbn isbn = ((Result<Isbn>)bookIsbnResult).Value!;
            var bookToCreate = new Book(
                book.BookTitle,
                ((Result<Edition>)bookEditionResult).Value!,
                isbn);
            isbns.Add(isbn.IsbnValue);
            booksToCreate.Add(bookToCreate);
            addBookToAuthor = newAuthor.AddBook(bookToCreate);
            if (!addBookToAuthor.IsSuccess)
            {
                break;
            }
        }
        var booksByIsbnsSpecification = new BooksByIsbnsSpecification(isbns);
        Book? existingBook = await _bookRepository.GetFirstOrDefaultBySpecificationAsync(booksByIsbnsSpecification);
        if (existingBook is not null)
        {
            bookAlreadyExists = Result.Fail(
                "bookAlreadyExists",
                $"A book with the ISBN '{existingBook.Isbn.IsbnValue}' already exists.");
        }
        Result combinedResults = Result.Combine(
            existingAuthorResult,
            authorFullNameResult,
            bookEditionResult,
            bookIsbnResult,
            bookAlreadyExists,
            addBookToAuthor);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<AuthorCreatedDto>(combinedResults.ErrorType, combinedResults.ErrorMessage);
        }
        await _authorRepository.CreateAsync(newAuthor, cancellationToken);
        return Result.Success(_authorToAuthorCreatedDtoMapper.Map(newAuthor));
    }
}
