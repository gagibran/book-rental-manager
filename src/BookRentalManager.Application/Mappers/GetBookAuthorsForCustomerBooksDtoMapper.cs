namespace BookRentalManager.Application.Mappers;

internal sealed class GetBookAuthorsForCustomerBooksDtoMapper
    : IMapper<IReadOnlyList<BookAuthor>, IReadOnlyList<GetBookAuthorsForCustomerBooksDto>>
{
    public IReadOnlyList<GetBookAuthorsForCustomerBooksDto> Map(
        IReadOnlyList<BookAuthor> bookAuthors
    )
    {
        if (bookAuthors is null)
        {
            return new List<GetBookAuthorsForCustomerBooksDto>();
        }
        IEnumerable<GetBookAuthorsForCustomerBooksDto> getBookAuthorsForCustomerBooksDto = from bookAuthor in bookAuthors
                                                                                           select new GetBookAuthorsForCustomerBooksDto(
                                                                                               bookAuthor.FullName
                                                                                           );
        return getBookAuthorsForCustomerBooksDto.ToList();
    }
}
