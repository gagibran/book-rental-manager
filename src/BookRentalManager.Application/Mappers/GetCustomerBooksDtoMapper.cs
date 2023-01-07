namespace BookRentalManager.Application.Mappers;

internal sealed class GetCustomerBooksDtoMapper : IMapper<IReadOnlyList<Book>, IReadOnlyList<GetCustomerBooksDto>>
{
    private readonly IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookAuthorsForCustomerBooksDto>> _getBookAuthorsForCustomerBooksDtoMapper;

    public GetCustomerBooksDtoMapper(
        IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookAuthorsForCustomerBooksDto>> getBookAuthorsForCustomerBooksDto
    )
    {
        _getBookAuthorsForCustomerBooksDtoMapper = getBookAuthorsForCustomerBooksDto;
    }

    public IReadOnlyList<GetCustomerBooksDto> Map(IReadOnlyList<Book> books)
    {
        if (books is null)
        {
            return new List<GetCustomerBooksDto>();
        }
        IEnumerable<GetCustomerBooksDto> getCustomerBooksDto = from book in books
                                                               select new GetCustomerBooksDto(
                                                                   book.BookTitle,
                                                                   _getBookAuthorsForCustomerBooksDtoMapper
                                                                       .Map(book.BookAuthors),
                                                                   book.Edition,
                                                                   book.Isbn
                                                               );
        return getCustomerBooksDto.ToList();
    }
}
