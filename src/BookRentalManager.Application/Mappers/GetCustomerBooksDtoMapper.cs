namespace BookRentalManager.Application.Mappers;

internal sealed class GetCustomerBooksDtoMapper : IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBookDto>>
{

    public GetCustomerBooksDtoMapper()
    {
    }

    public IReadOnlyList<GetCustomerBookDto> Map(IReadOnlyList<Book> books)
    {
        return (from book in books
                select new GetCustomerBookDto(
                    book.BookTitle,
                    book.Edition,
                    book.Isbn)
                ).ToList();
    }
}
