namespace BookRentalManager.Application.Mappers;

internal sealed class GetBookBookAuthorDtosMapper : IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookBookAuthorDto>>
{
    public IReadOnlyList<GetBookBookAuthorDto> Map(IReadOnlyList<BookAuthor> bookAuthors)
    {
        return (from bookAuthor in bookAuthors
                select new GetBookBookAuthorDto(bookAuthor.FullName)).ToList().AsReadOnly();
    }
}
