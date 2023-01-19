namespace BookRentalManager.Application.Mappers;

internal sealed class GetBookAuthorBooksDtoMapper : IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookAuthorBookDto>>
{
    public IReadOnlyList<GetBookAuthorBookDto> Map(IReadOnlyList<Book> books)
    {
        if (books is null)
        {
            return new List<GetBookAuthorBookDto>();
        }
        return (from book in books
                select new GetBookAuthorBookDto(
                    book.BookTitle,
                    book.Edition,
                    book.Isbn
                )).ToList();
    }
}
