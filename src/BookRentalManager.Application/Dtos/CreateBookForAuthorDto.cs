namespace BookRentalManager.Application.Dtos;

public sealed class CreateBookForAuthorDto
{
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public CreateBookForAuthorDto(
        string bookTitle,
        int edition,
        string isbn)
    {
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
    }
}
