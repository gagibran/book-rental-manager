namespace BookRentalManager.Application.Books.CommandHandlers;

internal sealed class CreateBookCommandHandler(IRepository<Book> bookRepository, IRepository<Author> authorRepository)
    : IRequestHandler<CreateBookCommand, BookCreatedDto>
{
    private readonly IRepository<Book> _bookRepository = bookRepository;
    private readonly IRepository<Author> _authorRepository = authorRepository;

    public async Task<Result<BookCreatedDto>> HandleAsync(CreateBookCommand createBookCommand, CancellationToken cancellationToken)
    {
        if (!createBookCommand.AuthorIds.Any())
        {
            return Result.Fail<BookCreatedDto>("missingAuthorIds", "'authorIds' is a required field");
        }
        var authorsByIdsSpecification = new AuthorsByIdsSpecification(createBookCommand.AuthorIds);
        IReadOnlyList<Author> authors = await _authorRepository.GetAllBySpecificationAsync(authorsByIdsSpecification, cancellationToken);
        Result authorsResult = Result.Success();
        if (authors.Count != createBookCommand.AuthorIds.Count())
        {
            authorsResult = Result.Fail("authorIds", "Could not find some of the authors for the provided IDs.");
        }
        Result<Edition> editionResult = Edition.Create(createBookCommand.Edition);
        Result<Isbn> isbnResult = Isbn.Create(createBookCommand.Isbn);
        Result combinedResults = Result.Combine(authorsResult, editionResult, isbnResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<BookCreatedDto>(combinedResults.ErrorType, combinedResults.ErrorMessage);
        }
        var newBook = new Book(createBookCommand.BookTitle, editionResult.Value!, isbnResult.Value!);
        foreach (Author author in authors)
        {
            author.AddBook(newBook);
        }
        await _bookRepository.CreateAsync(newBook, cancellationToken);
        return Result.Success(new BookCreatedDto(newBook));
    }
}
