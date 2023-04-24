namespace BookRentalManager.Application.Dtos;

public sealed class GetBookDto : SingleResourceBaseDto
{
    public string BookTitle { get; }
    public IReadOnlyList<GetAuthorFromBookDto> Authors { get; }
    public int Edition { get; }
    public string Isbn { get; }
    public DateTime? RentedAt { get; internal set; }
    public DateTime? DueDate { get; internal set; }
    public GetCustomerThatRentedBookDto RentedBy { get; }

    public GetBookDto(
        Guid id,
        string bookTitle,
        IReadOnlyList<GetAuthorFromBookDto> authors,
        Edition edition,
        Isbn isbn,
        DateTime? rentedAt,
        DateTime? dueDate,
        GetCustomerThatRentedBookDto rentedBy)
        : base(id)
    {
        BookTitle = bookTitle;
        Authors = authors;
        Edition = edition.EditionNumber;
        Isbn = isbn.IsbnValue;
        RentedAt = rentedAt;
        DueDate = dueDate;
        RentedBy = rentedBy;
    }
}
