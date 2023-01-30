namespace BookRentalManager.Application.Dtos;

public sealed class BookCreatedForAuthorDto
{
    public Guid Id { get; }
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public BookCreatedForAuthorDto(Guid id, string bookTitle, int edition, string isbn)
    {
        Id = id;
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
    }
}
