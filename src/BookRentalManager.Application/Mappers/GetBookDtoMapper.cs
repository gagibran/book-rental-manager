namespace BookRentalManager.Application.Mappers;

internal sealed class GetBookDtoMapper : IMapper<Book, GetBookDto>
{
    private IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>> _getAuthorFromBookDtosMapper;
    private IMapper<Customer?, GetRentedByDto> _getRentedByDtoMapper;

    public GetBookDtoMapper(
        IMapper<IReadOnlyList<Author>, IReadOnlyList<GetAuthorFromBookDto>> getAuthorFromBookDtosMapper,
        IMapper<Customer?, GetRentedByDto> getRentedByDtoMapper)
    {
        _getAuthorFromBookDtosMapper = getAuthorFromBookDtosMapper;
        _getRentedByDtoMapper = getRentedByDtoMapper;
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
            _getRentedByDtoMapper.Map(book.Customer));
    }
}
