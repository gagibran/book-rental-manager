namespace BookRentalManager.Application.Dtos;

public sealed class CreateBookDto
{
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public CreateBookDto(string bookTitle, int edition, string isbn)
    {
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
    }
}
