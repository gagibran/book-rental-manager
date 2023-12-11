namespace BookRentalManager.Application.Books.CommandHandlers;

internal sealed class PatchBookTitleEditionAndIsbnByIdCommandHandler(IRepository<Book> bookRepository)
    : IRequestHandler<PatchBookTitleEditionAndIsbnByIdCommand>
{
    private readonly IRepository<Book> _bookRepository = bookRepository;

    public async Task<Result> HandleAsync(
        PatchBookTitleEditionAndIsbnByIdCommand patchBookTitleEditionAndIsbnByIdCommand,
        CancellationToken cancellationToken)
    {
        var bookByIdWithAuthorsAndCustomersSpecification = new BookByIdWithAuthorsAndCustomersSpecification(
            patchBookTitleEditionAndIsbnByIdCommand.Id);
        Book? book = await _bookRepository.GetFirstOrDefaultBySpecificationAsync(
            bookByIdWithAuthorsAndCustomersSpecification,
            cancellationToken);
        if (book is null)
        {
            return Result.Fail<GetBookDto>(RequestErrors.IdNotFoundError, $"No book with the ID of '{patchBookTitleEditionAndIsbnByIdCommand.Id}' was found.");
        }
        if (book.Customer is not null)
        {
            return Result.Fail<GetBookDto>(
                "bookCustomer", $"This book is currently rented by {book.Customer.FullName}. Return the book before updating it.");
        }
        var patchBookTitleEditionAndIsbnByIdDto = new PatchBookTitleEditionAndIsbnByIdDto(book);
        Result patchAppliedResult = patchBookTitleEditionAndIsbnByIdCommand.PatchBookTitleEditionAndIsbnByIdDtoPatchDocument.ApplyTo(
            patchBookTitleEditionAndIsbnByIdDto,
            "add",
            "remove");
        Result updateBookTitleEditionAndIsbnResult = book.UpdateBookTitleEditionAndIsbn(
            patchBookTitleEditionAndIsbnByIdDto.BookTitle,
            patchBookTitleEditionAndIsbnByIdDto.Edition,
            patchBookTitleEditionAndIsbnByIdDto.Isbn);
        Result combinedResult = Result.Combine(patchAppliedResult, updateBookTitleEditionAndIsbnResult);
        if (!combinedResult.IsSuccess)
        {
            return combinedResult;
        }
        await _bookRepository.UpdateAsync(book, cancellationToken);
        return Result.Success();
    }
}
