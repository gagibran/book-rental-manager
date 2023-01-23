namespace BookRentalManager.Application.Dtos;

public sealed class GetBookAuthorBookDto
{
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public GetBookAuthorBookDto(string bookTitle, Edition edition, Isbn isbn)
    {
        BookTitle = bookTitle;
        Edition = edition.EditionNumber;
        Isbn = isbn.IsbnValue;
    }
}
