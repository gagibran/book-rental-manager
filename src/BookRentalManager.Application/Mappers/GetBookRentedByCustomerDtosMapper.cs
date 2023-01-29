namespace BookRentalManager.Application.Mappers;

internal sealed class GetBookRentedByCustomerDtosMapper : IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookRentedByCustomerDto>>
{
    public IReadOnlyList<GetBookRentedByCustomerDto> Map(IReadOnlyList<Book> books)
    {
        return (from book in books
                select new GetBookRentedByCustomerDto(
                    book.BookTitle,
                    book.Edition,
                    book.Isbn)
                ).ToList();
    }
}
