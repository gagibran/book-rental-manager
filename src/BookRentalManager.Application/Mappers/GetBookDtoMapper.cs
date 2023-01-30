namespace BookRentalManager.Application.Mappers;

internal sealed class GetBookDtoMapper : IMapper<Book, GetBookDto>
{
    private IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>> _getAuthorFromBookDtosMapper;
    private IMapper<Customer?, GetCustomerThatRentedBookDto> _getCustomerThatRentedBookDtoMapper;

    public GetBookDtoMapper(
        IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>> getAuthorFromBookDtosMapper,
        IMapper<Customer?, GetCustomerThatRentedBookDto> getCustomerThatRentedBookDtoMapper)
    {
        _getAuthorFromBookDtosMapper = getAuthorFromBookDtosMapper;
        _getCustomerThatRentedBookDtoMapper = getCustomerThatRentedBookDtoMapper;
    }

    public GetBookDto Map(Book book)
    {
        return new GetBookDto(
            book.Id,
            book.BookTitle,
            _getAuthorFromBookDtosMapper.Map(book.Authors),
            book.Edition,
            book.Isbn,
            book.IsAvailable,
            _getCustomerThatRentedBookDtoMapper.Map(book.Customer));
    }
}
