namespace BookRentalManager.Application.Dtos;

public sealed class BookCreatedDto(Guid id, string bookTitle, int edition, string isbn) : IdentifiableDto(id)
{
    public string BookTitle { get; } = bookTitle;
    public int Edition { get; } = edition;
    public string Isbn { get; } = isbn;
}
