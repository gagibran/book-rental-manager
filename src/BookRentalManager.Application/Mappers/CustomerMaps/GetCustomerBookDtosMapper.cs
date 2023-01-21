namespace BookRentalManager.Application.Mappers.CustomerMaps;

internal sealed class GetCustomerBookDtosMapper : IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBookDto>>
{

    public GetCustomerBookDtosMapper()
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
