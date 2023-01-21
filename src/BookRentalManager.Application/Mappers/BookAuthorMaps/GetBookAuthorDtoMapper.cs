namespace BookRentalManager.Application.Mappers.BookAuthorMaps;

internal sealed class GetBookAuthorDtoMapper : IMapper<BookAuthor, GetBookAuthorDto>
{
    private readonly IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookAuthorBookDto>> _getBookAuthorBooksDtoMapper;

    public GetBookAuthorDtoMapper(IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookAuthorBookDto>> getBookAuthorBooksDtoMapper)
    {
        _getBookAuthorBooksDtoMapper = getBookAuthorBooksDtoMapper;
    }

    public GetBookAuthorDto Map(BookAuthor bookAuthor)
    {
        return new GetBookAuthorDto(
            bookAuthor.Id,
            bookAuthor.FullName,
            _getBookAuthorBooksDtoMapper.Map(bookAuthor.Books));
    }
}
