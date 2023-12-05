namespace BookRentalManager.Application.DtoMappers;

internal sealed class BookToGetBookDtoMapper(IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>> authorsToGetAuthorFromBookDtosMapper)
    : IMapper<Book, GetBookDto>
{
    private readonly IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>> _authorsToGetAuthorFromBookDtosMapper = authorsToGetAuthorFromBookDtosMapper;

    public GetBookDto Map(Book book)
    {
        GetCustomerThatRentedBookDto? getCustomerThatRentedBookDto = null;
        if (book.Customer is not null)
        {
            getCustomerThatRentedBookDto = new GetCustomerThatRentedBookDto(book.Customer);
        }
        return new GetBookDto(
            book.Id,
            book.BookTitle,
            _authorsToGetAuthorFromBookDtosMapper.Map(book.Authors),
            book.Edition,
            book.Isbn,
            book.RentedAt,
            book.DueDate,
            getCustomerThatRentedBookDto);
    }
}
