namespace BookRentalManager.Application.Books.CommandHandlers;

internal sealed class PatchBookTitleEditionAndIsbnByIdCommandHandler : IRequestHandler<PatchBookTitleEditionAndIsbnByIdCommand>
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IMapper<Book, PatchBookTitleEditionAndIsbnByIdDto> _bookToPatchBookTitleEditionAndIsbnByIdDtoMapper;

    public PatchBookTitleEditionAndIsbnByIdCommandHandler(
        IRepository<Book> bookRepository,
        IMapper<Book, PatchBookTitleEditionAndIsbnByIdDto> bookToPatchBookTitleEditionAndIsbnByIdDtoMapper)
    {
        _bookRepository = bookRepository;
        _bookToPatchBookTitleEditionAndIsbnByIdDtoMapper = bookToPatchBookTitleEditionAndIsbnByIdDtoMapper;
    }

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
            return Result.Fail<GetBookDto>("bookId", $"No book with the ID of '{patchBookTitleEditionAndIsbnByIdCommand.Id}' was found.");
        }
        if (book.Customer is not null)
        {
            return Result.Fail<GetBookDto>(
                "bookCustomer", $"This book is currently rented by {book.Customer.FullName.ToString()}. Return the book before updating it.");
        }
        PatchBookTitleEditionAndIsbnByIdDto patchBookTitleEditionAndIsbnByIdDto = _bookToPatchBookTitleEditionAndIsbnByIdDtoMapper.Map(book);
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
