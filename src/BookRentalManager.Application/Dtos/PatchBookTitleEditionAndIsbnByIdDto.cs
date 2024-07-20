namespace BookRentalManager.Application.Dtos;

public sealed record PatchBookTitleEditionAndIsbnByIdDto(string BookTitle, int Edition, string Isbn)
{
    public PatchBookTitleEditionAndIsbnByIdDto(Book book)
        : this(book.BookTitle.Title, book.Edition.EditionNumber, book.Isbn.ToString())
    {
    }
}
