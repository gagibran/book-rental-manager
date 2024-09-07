namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBookByIdQueryHandler(IRepository<Book> bookRepository)
    : IRequestHandler<GetBookByIdQuery, GetBookDto>
{
    public async Task<Result<GetBookDto>> HandleAsync(GetBookByIdQuery getBookByIdQuery, CancellationToken cancellationToken)
    {
        var bookByIdWithAuthorsAndCustomersSpecification = new BookByIdWithAuthorsAndCustomersSpecification(getBookByIdQuery.Id);
        Book? book = await bookRepository.GetFirstOrDefaultBySpecificationAsync(bookByIdWithAuthorsAndCustomersSpecification, cancellationToken);
        if (book is null)
        {
            return Result.Fail<GetBookDto>(RequestErrors.IdNotFoundError, $"No book with the ID of '{getBookByIdQuery.Id}' was found.");
        }
        return Result.Success(new GetBookDto(book!));
    }
}
