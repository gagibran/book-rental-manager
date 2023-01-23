namespace BookRentalManager.Application.Mappers;

public sealed class GetBookDtoMapper : IMapper<Book, GetBookDto>
{
    private IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookBookAuthorDto>> _getBookBookAuthorDtosMapper;
    private IMapper<Customer?, GetRentedByDto> _getRentedByDtoMapper;

    public GetBookDtoMapper(
        IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookBookAuthorDto>> getBookBookAuthorDtosMapper,
        IMapper<Customer?, GetRentedByDto> getRentedByDtoMapper)
    {
        _getBookBookAuthorDtosMapper = getBookBookAuthorDtosMapper;
        _getRentedByDtoMapper = getRentedByDtoMapper;
    }

    public GetBookDto Map(Book book)
    {
        return new GetBookDto(
            book.Id,
            book.BookTitle,
            _getBookBookAuthorDtosMapper.Map(book.BookAuthors),
            book.Edition,
            book.Isbn,
            book.IsAvailable,
            _getRentedByDtoMapper.Map(book.Customer));
    }
}
