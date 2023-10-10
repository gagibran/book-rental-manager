namespace BookRentalManager.Application.DtoMappers;

internal sealed class BooksToGetBookRentedByCustomerDtosMapper : IMapper<IReadOnlyList<Book>, IReadOnlyList<GetBookRentedByCustomerDto>>
{
    public IReadOnlyList<GetBookRentedByCustomerDto> Map(IReadOnlyList<Book> books)
    {
        return (from book in books
                select new GetBookRentedByCustomerDto(
                    book.BookTitle,
                    book.Edition,
                    book.Isbn,
                    book.RentedAt!.Value,
                    book.DueDate!.Value)).ToList();
    }
}
