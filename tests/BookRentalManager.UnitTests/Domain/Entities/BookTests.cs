namespace BookRentalManager.UnitTests.Domain.Entities;

public sealed class BookTests
{
    private readonly Book _book;
    private readonly BookAuthor _bookAuthor;

    public BookTests()
    {
        _bookAuthor = new(FullName.Create("Eric", "Evans").Value);
        _book = new(
            "Domain-Driven Design: Tackling Complexity in the Heart of Software",
            Volume.Create(1).Value,
            Isbn.Create(9780321125217).Value
        );
    }

    [Fact]
    public void AddBookAuthor_WithExistingAuthor_ReturnsErrorMessage()
    {
        // Arrange:
        var bookAuthor = new BookAuthor(FullName.Create("Eric", "Evans").Value);
        var expectedErrorMessage = $"Eric Evans has already been added as an author to this book.";
        _book.AddBookAuthor(bookAuthor);

        // Act:
        Result addAuthorResult = _book.AddBookAuthor(bookAuthor);

        // Assert:
        Assert.Equal(expectedErrorMessage, addAuthorResult.ErrorMessage);
    }

    [Fact]
    public void AddBookAuthor_WithNonexistingAuthor_ReturnsAddedAuthor()
    {
        // Arrange:
        var author = new BookAuthor(FullName.Create("John", "Doe").Value);

        // Act:
        _book.AddBookAuthor(author);

        // Assert:
        Assert.Contains(author, _book.BookAuthors);
    }
}
