namespace BookRentalManager.Application.Books.CommandHandlers;

public sealed class DeleteBookByIdCommandHandler(IRepository<Book> bookRepository) : IRequestHandler<DeleteBookByIdCommand>
{
    private readonly IRepository<Book> _bookRepository = bookRepository;

    public async Task<Result> HandleAsync(DeleteBookByIdCommand deleteBookByIdCommand, CancellationToken cancellationToken)
    {
        var bookByIdWithAuthorsAndCustomersSpecification = new BookByIdWithAuthorsAndCustomersSpecification(deleteBookByIdCommand.Id);
        Book? book = await _bookRepository.GetFirstOrDefaultBySpecificationAsync(
            bookByIdWithAuthorsAndCustomersSpecification,
            cancellationToken);
        if (book is null)
        {
            return Result.Fail<GetBookDto>("bookId", $"No book with the ID of '{deleteBookByIdCommand.Id}' was found.");
        }
        if (book.Customer is not null)
        {
            return Result.Fail<GetBookDto>(
                "bookCustomer", $"This book is currently rented by {book.Customer.FullName}. Return the book before deleting it.");
        }
        await _bookRepository.DeleteAsync(book, cancellationToken);
        return Result.Success();
    }
}
