namespace BookRentalManager.Application.Dtos;

public sealed class PatchBookTitleEditionAndIsbnByIdDto
{
    public string BookTitle { get; set; }
    public int Edition { get; set; }
    public string Isbn { get; set; }

    public PatchBookTitleEditionAndIsbnByIdDto(string bookTitle, int edition, string isbn)
    {
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
    }
}
