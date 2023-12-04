namespace BookRentalManager.Application.Books.QueryHandlers;

internal sealed class GetBookByIdQueryHandler(IRepository<Book> bookRepository, IMapper<Book, GetBookDto> bookToGetBookDtoMapper)
    : IRequestHandler<GetBookByIdQuery, GetBookDto>
{
    private readonly IRepository<Book> _bookRepository = bookRepository;
    private readonly IMapper<Book, GetBookDto> _bookToGetBookDtoMapper = bookToGetBookDtoMapper;

    public async Task<Result<GetBookDto>> HandleAsync(GetBookByIdQuery getBookByIdQuery, CancellationToken cancellationToken)
    {
        var bookByIdWithAuthorsAndCustomersSpecification = new BookByIdWithAuthorsAndCustomersSpecification(getBookByIdQuery.Id);
        Book? book = await _bookRepository.GetFirstOrDefaultBySpecificationAsync(bookByIdWithAuthorsAndCustomersSpecification, cancellationToken);
        if (book is null)
        {
            return Result.Fail<GetBookDto>("bookId", $"No book with the ID of '{getBookByIdQuery.Id}' was found.");
        }
        return Result.Success(_bookToGetBookDtoMapper.Map(book!));
    }
}
