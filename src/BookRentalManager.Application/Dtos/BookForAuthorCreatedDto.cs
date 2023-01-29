namespace BookRentalManager.Application.Dtos;

public sealed class BookForAuthorCreatedDto
{
    public Guid Id { get; }
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public BookForAuthorCreatedDto(Guid id, string bookTitle, int edition, string isbn)
    {
        Id = id;
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
    }
}
