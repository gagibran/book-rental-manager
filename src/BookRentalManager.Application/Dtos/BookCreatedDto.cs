namespace BookRentalManager.Application.Dtos;

[method: JsonConstructor]
public sealed record BookCreatedDto(Guid Id, string BookTitle, int Edition, string Isbn) : IdentifiableDto(Id)
{
    public BookCreatedDto(Book book) : this(book.Id, book.BookTitle, book.Edition.EditionNumber, book.Isbn.ToString())
    {
    }
}
