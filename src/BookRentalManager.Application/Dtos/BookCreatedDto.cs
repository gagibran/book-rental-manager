namespace BookRentalManager.Application.Dtos;

public sealed class BookCreatedDto : SingleResourceBaseDto
{
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public BookCreatedDto(Guid id, string bookTitle, int edition, string isbn) : base(id)
    {
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
    }
}
