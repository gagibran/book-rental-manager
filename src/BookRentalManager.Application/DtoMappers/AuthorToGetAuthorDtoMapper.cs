namespace BookRentalManager.Application.DtoMappers;

internal sealed class AuthorToGetAuthorDtoMapper(IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>> booksToGetBookFromAuthorDtosMapper)
    : IMapper<Author, GetAuthorDto>
{
    private readonly IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>> _booksToGetBookFromAuthorDtosMapper = booksToGetBookFromAuthorDtosMapper;

    public GetAuthorDto Map(Author author)
    {
        return new GetAuthorDto(
            author.Id,
            author.FullName,
            _booksToGetBookFromAuthorDtosMapper.Map(author.Books));
    }
}
