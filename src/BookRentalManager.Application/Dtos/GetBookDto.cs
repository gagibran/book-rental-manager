namespace BookRentalManager.Application.Dtos;

public sealed class GetBookDto
{
    public Guid Id { get; }
    public string BookTitle { get; }
    public IReadOnlyList<GetAuthorFromBookDto> Authors { get; }
    public int Edition { get; }
    public string Isbn { get; }
    public bool IsAvailable { get; internal set; }
    public GetRentedByDto RentedBy { get; }

    public GetBookDto(
        Guid id,
        string bookTitle,
        IReadOnlyList<GetAuthorFromBookDto> authors,
        Edition edition,
        Isbn isbn,
        bool isAvailable,
        GetRentedByDto rentedBy)
    {
        Id = id;
        BookTitle = bookTitle;
        Authors = authors;
        Edition = edition.EditionNumber;
        Isbn = isbn.IsbnValue;
        IsAvailable = isAvailable;
        RentedBy = rentedBy;
    }
}
