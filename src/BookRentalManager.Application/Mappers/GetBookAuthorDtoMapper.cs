namespace BookRentalManager.Application.Mappers;

internal sealed class GetBookAuthorDtoMapper : IMapper<BookAuthor, GetBookAuthorDto>
{
    public GetBookAuthorDto Map(BookAuthor bookAuthor)
    {
        return new GetBookAuthorDto(bookAuthor.Id, bookAuthor.FullName);
    }
}
