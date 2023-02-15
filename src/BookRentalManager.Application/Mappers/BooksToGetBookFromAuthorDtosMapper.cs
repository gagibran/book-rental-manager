namespace BookRentalManager.Application.Mappers;

internal sealed class BooksToGetBookFromAuthorDtosMapper : IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookFromAuthorDto>>
{
    public IReadOnlyList<GetBookFromAuthorDto> Map(IReadOnlyList<Book> books)
    {
        return (from book in books
                select new GetBookFromAuthorDto(
                    book.BookTitle,
                    book.Edition,
                    book.Isbn)).ToList();
    }
}
