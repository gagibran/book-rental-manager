namespace BookRentalManager.Application.Dtos;

public sealed class GetBookFromAuthorDto(string bookTitle, Edition edition, Isbn isbn)
{
    public string BookTitle { get; } = bookTitle;
    public int Edition { get; } = edition.EditionNumber;
    public string Isbn { get; } = isbn.IsbnValue;
}
