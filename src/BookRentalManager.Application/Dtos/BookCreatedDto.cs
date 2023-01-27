namespace BookRentalManager.Application.Dtos;

public sealed class BookCreatedDto
{
    public Guid Id { get; }
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public BookCreatedDto(Guid id, string bookTitle, int edition, string isbn)
    {
        Id = id;
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
    }
}
