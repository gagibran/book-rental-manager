namespace BookRentalManager.Application.Dtos;

[method: JsonConstructor]
public sealed record GetBookFromAuthorDto(string BookTitle, int Edition, string Isbn)
{
    public GetBookFromAuthorDto(Book book) : this(
        book.BookTitle.Title,
        book.Edition.EditionNumber,
        book.Isbn.ToString())
    {
    }
}
