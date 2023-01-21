namespace BookRentalManager.Application.Dtos.BookDtos;

public sealed class GetBookDto
{
    public string BookTitle { get; }
    public IReadOnlyList<GetBookBookAuthorDto> BookAuthors { get; }
    public int Edition { get; }
    public string Isbn { get; }
    public bool IsAvailable { get; internal set; }
    public GetRentedByDto RentedBy { get; }

    public GetBookDto(
        string bookTitle,
        IReadOnlyList<GetBookBookAuthorDto> bookAuthors,
        Edition edition,
        Isbn isbn,
        bool isAvailable,
        GetRentedByDto rentedBy)
    {
        BookTitle = bookTitle;
        BookAuthors = bookAuthors;
        Edition = edition.EditionNumber;
        Isbn = isbn.IsbnValue;
        RentedBy = rentedBy;
    }
}
