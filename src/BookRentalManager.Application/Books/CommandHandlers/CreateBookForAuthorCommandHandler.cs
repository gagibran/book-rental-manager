namespace BookRentalManager.Application.Books.CommandHandlers;

internal sealed class CreateBookForAuthorCommandHandler : ICommandHandler<CreateBookForAuthorCommand, BookCreatedForAuthorDto>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Book, BookCreatedForAuthorDto> _bookToBookCreatedForAuthorDtoMapper;

    public CreateBookForAuthorCommandHandler(
        IRepository<Book> bookRepository,
        IRepository<Author> authorRepository,
        IMapper<Book, BookCreatedForAuthorDto> bookToBookCreatedForAuthorDtoMapper)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _bookToBookCreatedForAuthorDtoMapper = bookToBookCreatedForAuthorDtoMapper;
    }

    public async Task<Result<BookCreatedForAuthorDto>> HandleAsync(
        CreateBookForAuthorCommand createBookForAuthorCommand,
        CancellationToken cancellationToken)
    {
        var authorByIdWithBooksSpecification = new AuthorByIdWithBooksSpecification(createBookForAuthorCommand.AuthorId);
        Author? existingAuthor = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByIdWithBooksSpecification);
        Result existingAuthorResult = Result.Success();
        if (existingAuthor is null)
        {
            existingAuthorResult = Result.Fail("authorId", $"No author with the ID of '{createBookForAuthorCommand.AuthorId}' was found.");
        }
        Result<Edition> editionResult = Edition.Create(createBookForAuthorCommand.Edition);
        Result<Isbn> isbnResult = Isbn.Create(createBookForAuthorCommand.Isbn);
        Result combinedResults = Result.Combine(existingAuthorResult, editionResult, isbnResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<BookCreatedForAuthorDto>(combinedResults.ErrorType, combinedResults.ErrorMessage);
        }
        var newBook = new Book(createBookForAuthorCommand.BookTitle, editionResult.Value!, isbnResult.Value!);
        existingAuthor!.AddBook(newBook);
        await _bookRepository.CreateAsync(newBook, cancellationToken);
        return Result.Success<BookCreatedForAuthorDto>(_bookToBookCreatedForAuthorDtoMapper.Map(newBook));
    }
}
