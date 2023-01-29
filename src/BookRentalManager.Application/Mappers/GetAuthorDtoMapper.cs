namespace BookRentalManager.Application.Mappers;

internal sealed class GetAuthorDtoMapper : IMapper<Author, GetAuthorDto>
{
    private readonly IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>> _getBookFromAuthorDtosMapper;

    public GetAuthorDtoMapper(IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>> getBookFromAuthorDtosMapper)
    {
        _getBookFromAuthorDtosMapper = getBookFromAuthorDtosMapper;
    }

    public GetAuthorDto Map(Author author)
    {
        return new GetAuthorDto(
            author.Id,
            author.FullName,
            _getBookFromAuthorDtosMapper.Map(author.Books));
    }
}
