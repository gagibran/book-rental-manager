namespace BookRentalManager.Application.Dtos;

public sealed record GetBookFromAuthorDto(string BookTitle, int Edition, string Isbn)
{
    public GetBookFromAuthorDto(Book book) : this(
        book.BookTitle,
        book.Edition.EditionNumber,
        book.Isbn.ToString())
    {
    }
}
