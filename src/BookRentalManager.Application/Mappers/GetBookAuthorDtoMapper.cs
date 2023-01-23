namespace BookRentalManager.Application.Mappers;

internal sealed class GetBookAuthorDtoMapper : IMapper<BookAuthor, GetBookAuthorDto>
{
    private readonly IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookAuthorBookDto>> _getBookAuthorBookDtosMapper;

    internal GetBookAuthorDtoMapper(IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookAuthorBookDto>> getBookAuthorBookDtosMapper)
    {
        _getBookAuthorBookDtosMapper = getBookAuthorBookDtosMapper;
    }

    public GetBookAuthorDto Map(BookAuthor bookAuthor)
    {
        return new GetBookAuthorDto(
            bookAuthor.Id,
            bookAuthor.FullName,
            _getBookAuthorBookDtosMapper.Map(bookAuthor.Books));
    }
}
