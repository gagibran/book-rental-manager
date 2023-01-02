namespace BookRentalManager.Application.Mappers;

internal sealed class GetCustomerBooksDtoMapper : IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBooksDto>>
{
    private readonly IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookAuthorsForCustomerBooksDto>> _getBookAuthorsForCustomerBooksDto;

    public GetCustomerBooksDtoMapper(
        IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookAuthorsForCustomerBooksDto>> getBookAuthorsForCustomerBooksDto
    )
    {
        _getBookAuthorsForCustomerBooksDto = getBookAuthorsForCustomerBooksDto;
    }

    public IReadOnlyList<GetCustomerBooksDto> Map(IReadOnlyList<Book> books)
    {
        IEnumerable<GetCustomerBooksDto> getCustomerBooksDto = from book in books
                                                               select new GetCustomerBooksDto(
                                                                   book.BookTitle,
                                                                   _getBookAuthorsForCustomerBooksDto.Map(book.BookAuthors),
                                                                   book.Volume,
                                                                   book.Isbn
                                                               );
        return getCustomerBooksDto.ToList();
    }
}
