namespace BookRentalManager.Application.Dtos;

public sealed class GetCustomerBooksDto
{
    public string BookTitle { get; }
    public IReadOnlyList<GetBookAuthorsForCustomerBooksDto> BookAuthors { get; }
    public Volume Volume { get; }
    public Isbn Isbn { get; }

    public GetCustomerBooksDto(
        string bookTitle,
        IReadOnlyList<GetBookAuthorsForCustomerBooksDto> bookAuthors,
        Volume volume,
        Isbn isbn
    )
    {
        BookTitle = bookTitle;
        BookAuthors = bookAuthors;
        Volume = volume;
        Isbn = isbn;
    }
}
