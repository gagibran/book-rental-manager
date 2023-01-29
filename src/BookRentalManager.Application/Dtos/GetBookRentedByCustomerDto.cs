namespace BookRentalManager.Application.Dtos;

public sealed class GetBookRentedByCustomerDto
{
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public GetBookRentedByCustomerDto(string bookTitle, Edition edition, Isbn isbn)
    {
        BookTitle = bookTitle;
        Edition = edition.EditionNumber;
        Isbn = isbn.IsbnValue;
    }
}
