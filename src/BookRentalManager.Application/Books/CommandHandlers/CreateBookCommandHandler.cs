namespace BookRentalManager.Application.Books.CommandHandlers;

internal sealed class CreateBookCommandHandler(IRepository<Book> bookRepository, IRepository<Author> authorRepository)
    : IRequestHandler<CreateBookCommand, BookCreatedDto>
{
    public async Task<Result<BookCreatedDto>> HandleAsync(CreateBookCommand createBookCommand, CancellationToken cancellationToken)
    {
        if (!createBookCommand.AuthorIds.Any())
        {
            return Result.Fail<BookCreatedDto>("missingAuthorIds", "'authorIds' is a required field");
        }
        var authorsByIdsSpecification = new AuthorsByIdsSpecification(createBookCommand.AuthorIds);
        IReadOnlyList<Author> authors = await authorRepository.GetAllBySpecificationAsync(authorsByIdsSpecification, cancellationToken);
        Result authorsResult = Result.Success();
        if (authors.Count != createBookCommand.AuthorIds.Count())
        {
            authorsResult = Result.Fail("authorIds", "Could not find some of the authors for the provided IDs.");
        }
        Result<BookTitle> bookTitleResult = BookTitle.Create(createBookCommand.BookTitle);
        Result<Edition> editionResult = Edition.Create(createBookCommand.Edition);
        Result<Isbn> isbnResult = Isbn.Create(createBookCommand.Isbn);
        Result combinedResults = Result.Combine(authorsResult, bookTitleResult, editionResult, isbnResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<BookCreatedDto>(combinedResults.ErrorType, combinedResults.ErrorMessage);
        }
        var newBook = new Book(bookTitleResult.Value!, editionResult.Value!, isbnResult.Value!);
        foreach (Author author in authors)
        {
            author.AddBook(newBook);
        }
        await bookRepository.CreateAsync(newBook, cancellationToken);
        return Result.Success(new BookCreatedDto(newBook));
    }
}
