namespace BookRentalManager.Application.DtoMappers;

internal sealed class BookToGetBookDtoMapper : IMapper<Book, GetBookDto>
{
    private IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>> _authorsToGetAuthorFromBookDtosMapper;
    private IMapper<Customer?, GetCustomerThatRentedBookDto> _customerToGetCustomerThatRentedBookDtoMapper;

    public BookToGetBookDtoMapper(
        IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>> authorsToGetAuthorFromBookDtosMapper,
        IMapper<Customer?, GetCustomerThatRentedBookDto> customerToGetCustomerThatRentedBookDtoMapper)
    {
        _authorsToGetAuthorFromBookDtosMapper = authorsToGetAuthorFromBookDtosMapper;
        _customerToGetCustomerThatRentedBookDtoMapper = customerToGetCustomerThatRentedBookDtoMapper;
    }

    public GetBookDto Map(Book book)
    {
        return new GetBookDto(
            book.Id,
            book.BookTitle,
            _authorsToGetAuthorFromBookDtosMapper.Map(book.Authors),
            book.Edition,
            book.Isbn,
            book.IsAvailable,
            _customerToGetCustomerThatRentedBookDtoMapper.Map(book.Customer));
    }
}
