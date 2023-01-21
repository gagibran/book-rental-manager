namespace BookRentalManager.Application.Mappers.BookAuthorMaps;

internal sealed class GetBookAuthorBookDtosMapper : IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookAuthorBookDto>>
{
    public IReadOnlyList<GetBookAuthorBookDto> Map(IReadOnlyList<Book> books)
    {
        return (from book in books
                select new GetBookAuthorBookDto(
                    book.BookTitle,
                    book.Edition,
                    book.Isbn)
                ).ToList();
    }
}
