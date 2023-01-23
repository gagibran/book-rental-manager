namespace BookRentalManager.Application.Dtos;

public sealed class GetCustomerBookDto
{
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public GetCustomerBookDto(string bookTitle, Edition edition, Isbn isbn)
    {
        BookTitle = bookTitle;
        Edition = edition.EditionNumber;
        Isbn = isbn.IsbnValue;
    }
}
