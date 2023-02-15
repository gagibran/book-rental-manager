namespace BookRentalManager.Application.Mappers;

internal sealed class AuthorToGetAuthorDtoMapper : IMapper<Author, GetAuthorDto>
{
    private readonly IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>> _booksToGetBookFromAuthorDtosMapper;

    public AuthorToGetAuthorDtoMapper(IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>> booksToGetBookFromAuthorDtosMapper)
    {
        _booksToGetBookFromAuthorDtosMapper = booksToGetBookFromAuthorDtosMapper;
    }

    public GetAuthorDto Map(Author author)
    {
        return new GetAuthorDto(
            author.Id,
            author.FullName,
            _booksToGetBookFromAuthorDtosMapper.Map(author.Books));
    }
}
