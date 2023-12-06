namespace BookRentalManager.Application.Dtos;

public sealed class PatchBookTitleEditionAndIsbnByIdDto(string bookTitle, int edition, string isbn)
{
    public string BookTitle { get; set; } = bookTitle;
    public int Edition { get; set; } = edition;
    public string Isbn { get; set; } = isbn;

    public PatchBookTitleEditionAndIsbnByIdDto(Book book)
        : this(book.BookTitle, book.Edition.EditionNumber, book.Isbn.ToString())
    {
    }
}
