namespace BookRentalManager.Application.DtoMappers;

internal sealed class AuthorToAuthorCreatedDtoMapper : IMapper<Author, AuthorCreatedDto>
{
    private readonly IMapper<Book, BookCreatedForAuthorDto> _bookToBookCreatedForAuthorDtoMapper;

    public AuthorToAuthorCreatedDtoMapper(IMapper<Book, BookCreatedForAuthorDto> bookToBookCreatedForAuthorDtoMapper)
    {
        _bookToBookCreatedForAuthorDtoMapper = bookToBookCreatedForAuthorDtoMapper;
    }

    public AuthorCreatedDto Map(Author author)
    {
        return new AuthorCreatedDto(
            author.Id,
            author.FullName.CompleteName,
            author.Books.Select(book => _bookToBookCreatedForAuthorDtoMapper.Map(book)).ToList().AsReadOnly());
    }
}
