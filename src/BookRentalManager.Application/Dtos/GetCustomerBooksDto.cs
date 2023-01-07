namespace BookRentalManager.Application.Dtos;

public sealed class GetCustomerBooksDto
{
    public string BookTitle { get; }
    public IReadOnlyList<GetBookAuthorsForCustomerBooksDto> BookAuthors { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public GetCustomerBooksDto(
        string bookTitle,
        IReadOnlyList<GetBookAuthorsForCustomerBooksDto> bookAuthors,
        Edition edition,
        Isbn isbn
    )
    {
        BookTitle = bookTitle;
        BookAuthors = bookAuthors;
        Edition = edition.EditionNumber;
        Isbn = isbn.IsbnValue;
    }
}
